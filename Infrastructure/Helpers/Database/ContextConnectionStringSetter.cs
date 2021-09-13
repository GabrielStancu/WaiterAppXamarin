﻿using Core.Context;
using Infrastructure.Helpers.Parameters;

namespace Infrastructure.Helpers.Database
{
    public class ContextConnectionStringSetter
    {
        public void SetConnectionString()
        {
            var server = ParametersLoader.Parameters[AppParameters.Server];
            var database = ParametersLoader.Parameters[AppParameters.Database];
            var user = ParametersLoader.Parameters[AppParameters.DbUser];
            var password = ParametersLoader.Parameters[AppParameters.DbPassword];

            var connStrBuilder = new ConnectionStringBuilder(server, database, user, password);
            RestaurantContext.ConnectionString = connStrBuilder.Build();
        }
    }
}
