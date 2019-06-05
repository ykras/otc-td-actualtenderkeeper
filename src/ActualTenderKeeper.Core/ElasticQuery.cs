using System;
using Newtonsoft.Json;

namespace ActualTenderKeeper.Core
{
    public static class ElasticQuery
    {
        public static string NotActualTendersQueryJson =>
            JsonConvert.SerializeObject(
                    new
                    {
                        range = new
                        {
                            applicationEndDate = new
                            {
                                lt = DateTime.Now.ToUniversalTime()
                            }
                        }
                    }
                );
    }
}