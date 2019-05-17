using Microsoft.ML.Data;


namespace Facebook
{
    public class Item
    {
        //[Microsoft.ML.Runtime.Api.Column("0")]
        public int Id { get; set; }

        //[Microsoft.ML.Runtime.Api.Column("1")]
        public string FirstName { get; set; }

        //[Microsoft.ML.Runtime.Api.Column("2")]
        public string LastName { get; set; }

        //[Microsoft.ML.Runtime.Api.Column("3")]
        public string Description { get; set; }
    }
    public class ItemPrediction
    {
        //[ColumnName("Description")]
        public string Prediction;
    }
}