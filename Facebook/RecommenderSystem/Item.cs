using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;


namespace Facebook
{
    public class Item
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> KeywordList { get; set; }

        private static readonly string _Traindatapath = Path.Combine(Environment.CurrentDirectory, "Data", "StockTrain.csv");
        private static readonly string _Evaluatedatapath = Path.Combine(Environment.CurrentDirectory, "Data", "StockTest.csv");
        public Lazy<List<Item>> _items = new Lazy<List<Item>>(() => LoadUsersData(_Evaluatedatapath));

        public Item()
        {
        }

        public Item Get(int id)
        {
            return _items.Value.Single(m => m.Id == id);
        }

        private static List<Item> LoadUsersData(String usersdatasetpath)
        {
            var result = new List<Item>();
            Stream fileReader = File.OpenRead(usersdatasetpath);
            StreamReader reader = new StreamReader(fileReader);
            try
            {
                bool header = true;
                int index = 0;
                var line = "";
                while (!reader.EndOfStream)
                {
                    if (header)
                    {
                        line = reader.ReadLine();
                        header = false;
                    }
                    line = reader.ReadLine();
                    string[] fields = line.Split(',');
                    int userId = Int32.Parse(fields[0].ToString().TrimStart(new char[] { '0' }));
                    string userFirstName = fields[1].ToString();
                    string userLastName = fields[2].ToString();
                    List<string> keywordList = fields[3].ToString().Split(' ').ToList();
                    result.Add(new Item() {
                        Id = userId,
                        FirstName = userFirstName,
                        LastName = userLastName,
                        KeywordList = keywordList
                    });
                    index++;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            return result;
        }
    }
}

   /* public class ItemPrediction
    {
        [ColumnName("Description")]
        public string Prediction;
    }*/