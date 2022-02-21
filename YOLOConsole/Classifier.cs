using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Drawing;
using System.Windows.Media.Imaging;
using YOLOConsole.DataStructures;

namespace YOLOConsole
{
    public static class Classifier
    {
        const string modelPath = @"C:\Users\kuris\Documents\GitHub\yolov4.onnx";
        static readonly string[] classesNames = new string[] { "person", "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa", "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };

        public static async Task ClassifyAsync(string imageFolder, CancellationToken token, BufferBlock<IReadOnlyList<YoloV4Result>> resultBuffer)
        {
            MLContext mlContext = new MLContext();

            // Define scoring pipeline
            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                    },
                    inputColumnNames: new[]
                    {
                        "input_1:0"
                    },
                    outputColumnNames: new[]
                    {
                        "Identity:0",
                        "Identity_1:0",
                        "Identity_2:0"
                    },
                    modelFile: modelPath, recursionLimit: 100));

            // Fit on empty list to obtain input data schema
            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));

            var predictingActionBlock = new ActionBlock<string>(
                async imageName =>
                {
                    if (token.IsCancellationRequested)
                        return;
                    await Task.Run(() =>
                    {
                        using (var bitmap = new Bitmap(Image.FromFile(imageName)))
                        {
                            if (token.IsCancellationRequested) return;
                            var predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
                            var predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });

                            if (token.IsCancellationRequested) return;
                            var results = predict.GetResults(classesNames, 0.3f, 0.7f);

                            foreach (var item in results)
                                item.SetImgName(imageName);

                            resultBuffer.Post(results);
                        }
                    }
                    );
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }
            );

            string[] imageNames = Directory.GetFiles(imageFolder, "*.jpg");
            foreach (string item in imageNames)
                predictingActionBlock.Post(item);
            predictingActionBlock.Complete();
            await predictingActionBlock.Completion;
            resultBuffer.Complete();
        }
    }
}
