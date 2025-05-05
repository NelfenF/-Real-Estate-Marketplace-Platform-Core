﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeListingAPI
{
    internal static class WriteHome
    {
        internal static int CreateNew(Home home)
        {
            DBConnect dbConnect = new DBConnect();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "P4_CreateNewHome";

            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@agentID", (int)home.AgentID, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@cost", home.Cost, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<byte[]>("@homeAddress", Serializer.SerializeData<Address>(home.Address), SqlDbType.VarBinary));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@propertyType", home.PropertyType.ToString(), SqlDbType.VarChar, 50));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@homeSize", home.HomeSize, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@constructionYear", home.YearConstructed, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@garage", home.GarageType.ToString(), SqlDbType.VarChar, 50));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@homeDescription", home.Description, SqlDbType.VarChar));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<DateTime>("@dateListed", home.DateListed, SqlDbType.DateTime));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@saleStatus", home.SaleStatus.ToString(), SqlDbType.VarChar, 50));

            SqlParameter outputParam = DBParameterHelper.OutputParameter("@homeID", SqlDbType.Int, 8);
            sqlCommand.Parameters.Add(outputParam);

            dbConnect.DoUpdate(sqlCommand);
            return (int)outputParam.Value;
        }
        internal static bool UpdateSaleStatus(Home home)
        {
            DBConnect dbConnect = new DBConnect();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "P4_UpdateHomeSaleStatus";

            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@homeID", (int)home.HomeID, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@saleStatus", home.SaleStatus.ToString(), SqlDbType.VarChar, 50));

            SqlParameter outputParam = DBParameterHelper.OutputParameter("@changed", SqlDbType.Int, 8);
            sqlCommand.Parameters.Add(outputParam);

            dbConnect.DoUpdate(sqlCommand);
            return (int)outputParam.Value == 0;
        }
        internal static void DeleteHome(int homeID)
        {
			try
			{
				DBConnect dbConnect = new DBConnect();
				SqlCommand sqlCommand = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					CommandText = "P4_DeleteHome"
				};
				sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@HomeID", homeID, SqlDbType.Int, 8));

				dbConnect.DoUpdate(sqlCommand);
			}
			catch (SqlException sqlEx)
			{
				// Handle SQL specific exceptions, like issues with query, constraints, etc.
				Console.WriteLine($"SQL Exception: {sqlEx.Message}");
				throw; // Rethrow if you want it to bubble up or handle it differently
			}
			catch (Exception ex)
			{
				// Handle all other types of exceptions
				Console.WriteLine($"Exception: {ex.Message}");
				throw; // Rethrow the exception to be handled at a higher level or log it
			}

		}

        internal static void UpdateHome(Home home, Home oldHome)
        {
            DBConnect dbConnect = new DBConnect();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "P4_UpdateHomeListing";
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@HomeID", (int)home.HomeID, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@Cost", home.Cost, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<byte[]>("@HomeAddress", Serializer.SerializeData<Address>(home.Address), SqlDbType.VarBinary));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@PropertyType", home.PropertyType.ToString(), SqlDbType.VarChar, 50));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@HomeSize", home.HomeSize, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@ConstructionYear", home.YearConstructed, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@Garage", home.GarageType.ToString(), SqlDbType.VarChar, 50));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@HomeDescription", home.Description, SqlDbType.VarChar));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<DateTime>("@DateListed", home.DateListed, SqlDbType.DateTime));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@SaleStatus", home.SaleStatus.ToString(), SqlDbType.VarChar, 50));
            dbConnect.DoUpdate(sqlCommand);

            WriteAmenity.UpdateAmenities((int)home.HomeID, home.Amenities, oldHome.Amenities);
            WriteHomeImage.UpdateHomeImages((int)home.HomeID, home.Images, oldHome.Images);
            WriteRoom.UpdateRooms((int)home.HomeID, home.Rooms, oldHome.Rooms);
            WriteTemperatureControl.UpdateTemperatureControl((int)home.HomeID, home.TemperatureControl);
            WriteUtility.UpdateUtilities((int)home.HomeID, home.Utilities, oldHome.Utilities);

        }
    }
}
