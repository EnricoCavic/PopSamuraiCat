<#
.SYNOPSIS
    Reports how much of the conversation context this session is using, from the session transcript.

.DESCRIPTION
    Claude Code writes a JSONL transcript per session under
    %USERPROFILE%\.claude\projects\<slugified-cwd>\<session-id>.jsonl. Every assistant message
    carries a `usage` block. The CONTEXT SIZE at any point is the last main-thread assistant
    message's input_tokens + cache_read_input_tokens + cache_creation_input_tokens -- those three
    are what was actually sent to the model for that turn.

    Two modes:
      -Hook    Prints ONE line, and only when the context is at or past -WarnAt. Silent otherwise,
               so it costs nothing until it matters. This is what the method plugin's
               hooks/hooks.json runs, as its UserPromptSubmit hook.
      (none)   Full report: context size, per-model output tokens, and the main-thread vs subagent
               split for this session.

    WHAT THIS CANNOT SEE: account-level daily/weekly usage, and spend across sessions or projects.
    Those live in the Claude usage dashboard, not on disk. Everything here is one session's
    transcript. See wiki/skills/handing-off-a-session.md.

.PARAMETER TranscriptPath
    Path to a session .jsonl. Defaults to the newest transcript for the current project.

.PARAMETER WarnAt
    Context-token threshold. Default 150000; the ceiling is set in wiki/SCHEMA.md.

.PARAMETER Hook
    Hook mode. Reads the hook's JSON payload from stdin (for `transcript_path`) if one is piped in,
    and prints only the over-threshold line.

.EXAMPLE
    powershell -NoProfile -File tools\context-report.ps1
#>
[CmdletBinding()]
param(
    [string]$TranscriptPath,
    [int]$WarnAt = 150000,
    [switch]$Hook
)

$ErrorActionPreference = 'Stop'

function Get-ProjectTranscriptDir {
    # Claude Code slugifies the working directory: "c:\Users\x\y" -> "c--Users-x-y"
    $slug = (Get-Location).Path.Replace(':', '-').Replace('\', '-').Replace('/', '-')
    return (Join-Path $env:USERPROFILE ".claude\projects\$slug")
}

# --- Resolve the transcript ------------------------------------------------------------------

if ($Hook -and -not $TranscriptPath) {
    # Hooks receive a JSON payload on stdin. It is documented to carry `transcript_path`; if that
    # ever stops being true, the newest-file fallback below still finds the right file.
    if (-not [Console]::IsInputRedirected) {
        $stdin = ''
    } else {
        $stdin = [Console]::In.ReadToEnd()
    }
    if ($stdin -and $stdin.Trim().StartsWith('{')) {
        try {
            $payload = $stdin | ConvertFrom-Json
            if ($payload.transcript_path) { $TranscriptPath = $payload.transcript_path }
        } catch {
            # Malformed payload is not worth failing a hook over; fall through to the fallback.
        }
    }
}

if (-not $TranscriptPath) {
    $dir = Get-ProjectTranscriptDir
    if (-not (Test-Path $dir)) {
        if ($Hook) { exit 0 }
        Write-Error "No transcript directory for this project: $dir"
    }
    $newest = Get-ChildItem -Path $dir -Filter '*.jsonl' -File |
              Sort-Object LastWriteTime -Descending |
              Select-Object -First 1
    if (-not $newest) {
        if ($Hook) { exit 0 }
        Write-Error "No .jsonl transcripts in $dir"
    }
    $TranscriptPath = $newest.FullName
}

if (-not (Test-Path $TranscriptPath)) {
    if ($Hook) { exit 0 }
    Write-Error "Transcript not found: $TranscriptPath"
}

# --- Context size: the newest main-thread assistant message with a usage block ----------------

function Get-ContextSize {
    param([string]$Path)

    # Read from the end -- the answer is almost always in the last few records.
    foreach ($take in 200, 2000, 100000) {
        $lines = Get-Content -Path $Path -Tail $take -Encoding UTF8
        for ($i = $lines.Count - 1; $i -ge 0; $i--) {
            $line = $lines[$i]
            if (-not $line) { continue }
            try { $rec = $line | ConvertFrom-Json } catch { continue }
            if ($rec.type -ne 'assistant') { continue }
            if ($rec.isSidechain -eq $true) { continue }   # subagent turn, not our context
            $u = $rec.message.usage
            if (-not $u) { continue }
            return [pscustomobject]@{
                Tokens    = [int]$u.input_tokens + [int]$u.cache_read_input_tokens + [int]$u.cache_creation_input_tokens
                Model     = $rec.message.model
                Timestamp = $rec.timestamp
            }
        }
        if ($lines.Count -lt $take) { break }   # we already read the whole file
    }
    return $null
}

$ctx = Get-ContextSize -Path $TranscriptPath
if (-not $ctx) {
    if ($Hook) { exit 0 }
    Write-Error "No assistant message with usage data in $TranscriptPath"
}

# --- Hook mode: one line, only when it matters -------------------------------------------------

if ($Hook) {
    if ($ctx.Tokens -ge $WarnAt) {
        $k = [math]::Round($ctx.Tokens / 1000)
        Write-Output "[context] ~${k}k tokens, at or past the ${WarnAt} ceiling. Land the current deliverable, update the task file's ## State so it can be resumed cold, commit, then tell the human to /clear."
    }
    exit 0
}

# --- Full report -------------------------------------------------------------------------------

$byModel   = @{}
$mainOut   = 0
$sideOut   = 0
$turns     = 0

foreach ($line in [System.IO.File]::ReadLines($TranscriptPath)) {
    if (-not $line) { continue }
    try { $rec = $line | ConvertFrom-Json } catch { continue }
    if ($rec.type -ne 'assistant') { continue }
    $u = $rec.message.usage
    if (-not $u) { continue }
    $turns++
    $model = $rec.message.model
    if (-not $model) { $model = 'unknown' }
    if (-not $byModel.ContainsKey($model)) { $byModel[$model] = 0 }
    $byModel[$model] += [int]$u.output_tokens
    if ($rec.isSidechain -eq $true) { $sideOut += [int]$u.output_tokens }
    else                            { $mainOut += [int]$u.output_tokens }
}

$pct = [math]::Round(100 * $ctx.Tokens / $WarnAt)

# Format with invariant culture -- a locale that groups with "." renders 150000 as "150.000",
# which reads as 150 in a report whose whole subject is a token count.
function Format-Num { param([int]$n) return $n.ToString('N0', [cultureinfo]::InvariantCulture) }

Write-Output ""
Write-Output "Session transcript : $(Split-Path $TranscriptPath -Leaf)"
Write-Output "Last turn          : $($ctx.Timestamp)  ($($ctx.Model))"
Write-Output ""
Write-Output "CONTEXT NOW        : $(Format-Num $ctx.Tokens) tokens   ($pct% of the $(Format-Num $WarnAt) ceiling)"
if ($ctx.Tokens -ge $WarnAt) {
    Write-Output "                     >>> at or past the ceiling -- land the deliverable and /clear"
}
Write-Output ""
Write-Output "Output tokens this session ($turns assistant turns):"
foreach ($m in ($byModel.Keys | Sort-Object)) {
    Write-Output ("  {0,-34} {1,10}" -f $m, (Format-Num $byModel[$m]))
}
Write-Output ("  {0,-34} {1,10}" -f '-- main thread', (Format-Num $mainOut))
Write-Output ("  {0,-34} {1,10}" -f '-- subagents (sidechain)', (Format-Num $sideOut))
Write-Output ""
Write-Output "Account-level daily/weekly usage is NOT here -- it is only in the Claude usage dashboard."
Write-Output ""
