using System;
using System.IO;
using Facebook;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.Data.DataView;

namespace MLNETStock
{
    class Program
    {
        //MLContext mlContext = new MLContext(seed: 0);
        static readonly string _Traindatapath = Path.Combine(Environment.CurrentDirectory, "Data", "StockTrain.csv");
        static readonly string _Testdatapath = Path.Combine(Environment.CurrentDirectory, "Data", "StockTest.csv");
        static readonly string _UsersDataLocation = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");

        static void Main(string[] args)
        {
            //STEP 1: Create MLContext to be shared across the model creation workflow objects 
            MLContext mlcontext = new MLContext();

            //STEP 2: Read the training data which will be used to train the movie recommendation model    
            //The schema for training data is defined by type 'TInput' in LoadFromTextFile<TInput>() method.
            IDataView trainingDataView = mlcontext.Data.LoadFromTextFile<UserRecommendation>(_Traindatapath, hasHeader: true, separatorChar: ',');

            //STEP 3: Transform your data by encoding the two features user1Id and user2ID. These encoded features will be provided as input
            //        to our MatrixFactorizationTrainer.
            var dataProcessingPipeline = mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName: "user1IdEncoded", inputColumnName: nameof(UserRecommendation.user1Id))
                           .Append(mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName: "user2IdEncoded", inputColumnName: nameof(UserRecommendation.user2Id)));

        }




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
        /*
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
    */
    }
}
