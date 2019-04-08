using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ML.Data;

namespace Facebook
{
    public class UserRecommendation
    {
        [LoadColumn(0)]
        public float user1Id;

        [LoadColumn(1)]
        public float user2Id;

        [LoadColumn(2)]
        public float Label;
    }
}