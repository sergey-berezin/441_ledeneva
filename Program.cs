﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using YOLOv4MLNet.DataStructures;

namespace YOLOv4MLNet
{
    class Program
    {
        const string imageFolder = @"C:\Users\kuris\Documents\BEREZIN\YOLOv4MLNet\Assets\Images";

        //const string imageOutputFolder = @"C:\Users\kuris\Documents\BEREZIN\YOLOv4MLNet\Assets\Output";
        static async Task ClassesProcessingAsync(ISourceBlock<IReadOnlyList<YoloV4Result>> src, Dictionary<string, int> foundClasses)
        {
            int imagesCnt = Directory.GetFiles(imageFolder, "*.jpg").Length;
            Console.WriteLine("There are " + imagesCnt + " images\n");

            int i = 0;
            while (await src.OutputAvailableAsync())
            {
                var results = src.Receive();
                foreach (var item in results)
                {
                    if (foundClasses.ContainsKey(item.Label))
                        foundClasses[item.Label]++;
                    else
                        foundClasses.Add(item.Label, 1);
                }

                i++;
                Console.WriteLine(i * 100 / imagesCnt + "% of images processed...");
            }
        }

        static async Task Main()
        {
            var output = new BufferBlock<IReadOnlyList<YoloV4Result>>();
            var foundClasses = new Dictionary<string, int>();

            var tokenSrc = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Cancel\n");
                tokenSrc.Cancel();
                e.Cancel = false;
            };

            var result = Classifier.ClassifyAsync(imageFolder, tokenSrc.Token, output);

            await ClassesProcessingAsync(output, foundClasses);
            Console.WriteLine("\nList of found classes:\n");
            foreach (var item in foundClasses)
            {
                Console.WriteLine(item.Key + ": " + item.Value + " instances");
            }
        }
    }
}
