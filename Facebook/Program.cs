using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Facebook;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Models;

namespace MLNETStock
{
    class Program
    {
        //MLContext mlContext = new MLContext(seed: 0);
        static readonly string _Traindatapath = Path.Combine(Environment.CurrentDirectory, "Data", "StockTrain.csv");
        static readonly string _Evaluatedatapath = Path.Combine(Environment.CurrentDirectory, "Data", "StockTest.csv");
        static readonly string _modelpath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");


        /*static async Task Main(string[] args)
        {
            //var model = Train(mlContext: mlContext, dataPath: _Traindatapath);
            PredictionModel<Item, ItemPrediction> model = await TrainourModel();

            Evaluate(model);

            Console.WriteLine(" ");
            Console.WriteLine("-------First Prediction Vale : ----------------");
            Console.WriteLine(" ");
            ItemPrediction prediction = model.Predict(Item);
            Console.WriteLine("Item ID  {0}", Item.stock1.ItemID);

            Console.WriteLine("Predicted Stock: {0}, actual Stock Qty: 90 ", prediction.Prediction);
            //Console.WriteLine("Predicted Stock: {0},actual Stock Qty: 90 - > Round Predicted Value",  Math.Round(prediction.TotalStockQty));
            Console.WriteLine(" ");
            Console.WriteLine("----------Next Prediction : -------------");
            Console.WriteLine(" ");
            prediction = model.Predict(Item.stock2);
            Console.WriteLine("Item ID  {0}", Item.Id);

            Console.WriteLine("Predicted Stock: {0}, actual Stock Qty: 4800 ", prediction.TotalStockQty);
            //Console.WriteLine("Predicted Stock: {0}, actual Stock Qty: 4800  - > Round Predicted Value -> ", Math.Round(prediction.TotalStockQty));
            Console.ReadLine();
        }
       /* public static ITransformer Train(MLContext mlContext, string dataPath)
        {

        }*/

        public static async Task<PredictionModel<Item, ItemPrediction>> TrainourModel()
        {
            var pipeline = new LearningPipeline
             {
                new TextLoader(_Traindatapath).CreateFrom<Item>(useHeader: true, separator: ','),
                new ColumnCopier(("TotalStockQty", "Label")),
                new CategoricalOneHotVectorizer(
                    "ItemID",
                   "Loccode",
                  //"InQty",
                  //  "OutQty",
                    "ItemType"),
                new ColumnConcatenator(
                    "Features",
                    "ItemID",
                    "Loccode",
                   "InQty",
                    "OutQty",
                    "ItemType"),
                new FastTreeRegressor()
            };

            PredictionModel<Item, ItemPrediction> model = pipeline.Train<Item, ItemPrediction>();

            await model.WriteAsync(_modelpath);
            return model;
        }


        private static void Evaluate(PredictionModel<Item, ItemPrediction> model)
        {
            var testData = new TextLoader(_Evaluatedatapath).CreateFrom<Item>(useHeader: true, separator: ',');
            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, testData);

            Console.WriteLine($"Rms = {metrics.Rms}");

            Console.WriteLine($"RSquared = {metrics.RSquared}");
          
        }

    }
}
