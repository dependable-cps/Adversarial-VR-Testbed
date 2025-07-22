using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Barracuda;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Reflection;
using System.Linq;
public class Getinferencefrommodel 

{
    public DenseTensor <int> outputTensor;
    static string modelPath = Application.streamingAssetsPath + "/Models/stacked_clf.onnx";
    InferenceSession session = new InferenceSession(modelPath);
   // public NNModel modelAsset;
    //private Model m_RuntimeModel;
    //private IWorker m_Worker;
     //@"/home/trojan/unity/Projects/HeadLocation/best_model.onnx";
    // Start is called before the first frame update
    /*public void Start()
    {
        m_RuntimeModel = ModelLoader.Load(modelAsset);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, m_RuntimeModel);

    }*/

    // Update is called once per frame
    public long predict (float [] inputArray)
    {
            var inputTensor = new DenseTensor<float>(inputArray,
                                                         new int[] { 1, 11 });
                    var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("feature_input", inputTensor)
            };
       
         
        var sessionOutput = session.Run(inputs);
        var rawResult = (DisposableNamedOnnxValue)sessionOutput.ToArray()[1];
        var onnxValue = (IDisposableReadOnlyCollection<DisposableNamedOnnxValue>)rawResult.Value;
        var probalities = (Dictionary<long, float>)onnxValue.ToArray()[0].Value;
        var rawlabel = (DisposableNamedOnnxValue)sessionOutput.ToArray()[0];
        var onnxlabel = rawlabel.Value as DenseTensor<long>;
        var probabilitynone = probalities.Values.ToArray()[0].ToString();
        var probabilitylow = probalities.Values.ToArray()[1].ToString();
        var probabilitymedium = probalities.Values.ToArray()[2].ToString();
        var probabilityhigh = probalities.Values.ToArray()[3].ToString();
        var responseMessage = $"Probabilities: \nNone: {probabilitynone}\n" +
                $"Low: {probabilitylow}\nMedium: {probabilitymedium}\nHigh: {probabilityhigh}";

        score.proba = responseMessage;
        //Debug.Log(onnxlabel.GetValue(0));
        //print the label
        //Console.WriteLine(onnxlabel.GetValue(0));
        /*
        foreach (var r in results)
        {
                        Debug.Log($"Output for { r.Name}");
                        Debug.Log(r.Value);
                        this.outputTensor = r.Value as DenseTensor <int>;
        }
        var output = results;
        //Debug.Log(output.ToString());*/
        score.Score = onnxlabel.GetValue(0);
        return onnxlabel.GetValue(0);
        /*if(m_Worker != null){
        m_Worker.Execute(inputTensor);
        Tensor output = m_Worker.PeekOutput();
        return output;}
        return inputTensor;*/
    }
    
}
