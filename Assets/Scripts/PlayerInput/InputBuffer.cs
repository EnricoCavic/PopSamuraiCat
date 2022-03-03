using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer
{
    //filas não são otimizadas para o problema, refatorar para outro tipo de dado
    public Queue inputQueue = new Queue();
    private WaitForSeconds wait;
    private MonoBehaviour coroutineStarter;

    public InputBuffer(MonoBehaviour _coroutineStarter, float _inputBufferTimer)
    {
        coroutineStarter = _coroutineStarter;
        wait = new WaitForSeconds(_inputBufferTimer);
    }

    public void RegisterInput(string _actionName)
    {
        InputData inputData = new InputData(_actionName, Time.frameCount);
        inputQueue.Enqueue(inputData);
        coroutineStarter.StartCoroutine(InputBufferTimeout(inputData));
    }

    public InputData RemoveInput()
    {
        if(CheckNextInputInBuffer())
            return inputQueue.Dequeue() as InputData;
        
        return null;
    }

    public bool CheckNextInputInBuffer(string _actionName)
    {   if(!CheckNextInputInBuffer())
            return false;

        InputData inputData = inputQueue.Peek() as InputData;        
        return inputData.actionName == _actionName;
    }

    public bool CheckNextInputInBuffer(InputData _data)
    {   if(!CheckNextInputInBuffer())
            return false;
       
        return inputQueue.Peek() == _data;
    }

    public bool CheckNextInputInBuffer()
    {          
        return inputQueue.Count > 0;
    }

    public IEnumerator InputBufferTimeout(InputData _data)
    {
        yield return wait;
        if(CheckNextInputInBuffer(_data))
        {
            inputQueue.Dequeue();
        }
    }
}
