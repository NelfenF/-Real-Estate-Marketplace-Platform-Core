﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeListingAPI
{
    internal class WriteAmenity
    {
        internal static int CreateNew(int homeID, Amenity amenity)
        {
            DBConnect dbConnect = new DBConnect();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "P4_CreateNewAmenity";

            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@homeID", (int)homeID, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@amenityType", amenity.Type.ToString(), SqlDbType.VarChar, 50));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@amenityDescription", amenity.Description, SqlDbType.VarChar));

            SqlParameter outputParam = DBParameterHelper.OutputParameter("@amenityID", SqlDbType.Int, 8);
            sqlCommand.Parameters.Add(outputParam);

            dbConnect.DoUpdate(sqlCommand);
            return (int)outputParam.Value;
        }

        internal static void DeleteAmenity(int homeID)
        {
			try
			{
				DBConnect dbConnect = new DBConnect();
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "P4_DeleteAmenity";
				sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@HomeID", (int)homeID, SqlDbType.Int, 8));
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

        internal static void UpdateAmenities(int homeID, Amenities updatedAmenities, Amenities oldAmenities)
        {

			//Remove deleted amenities
			foreach (Amenity currentOldAmenity in oldAmenities.List)
			{
				// Check if the current old amenity exists in the updated amenities list
				bool existsInUpdatedList = updatedAmenities.List.Any(updatedAmenity => updatedAmenity.AmenityID == currentOldAmenity.AmenityID);

				if (!existsInUpdatedList)
				{
					// Delete the old amenity because it wasn't present in the updated amenities
					DBConnect dbConnect = new DBConnect();
					SqlCommand sqlCommand = new SqlCommand
					{
						CommandType = CommandType.StoredProcedure,
						CommandText = "P4_RemoveAmenity"
					};
					sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@AmenityID", (int)currentOldAmenity.AmenityID, SqlDbType.Int, 8));
					dbConnect.DoUpdate(sqlCommand);
				}
			}


			foreach (Amenity currentAmenity in updatedAmenities.List)
            {
                if (currentAmenity.AmenityID != 0)
                {
                    DBConnect dbConnect = new DBConnect();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "P4_UpdateAmenity";
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@AmenityID", (int)currentAmenity.AmenityID, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@HomeID", homeID, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@AmenityType", currentAmenity.Type.ToString(), SqlDbType.VarChar, 50));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@AmenityDescription", currentAmenity.Description, SqlDbType.VarChar));
                    dbConnect.DoUpdate(sqlCommand);
                }
                else
                {
                    //Create new amenities
                    DBConnect dbConnect = new DBConnect();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "P4_CreateNewAmenity";
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@homeID", (int)homeID, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@amenityType", currentAmenity.Type.ToString(), SqlDbType.VarChar, 50));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@amenityDescription", currentAmenity.Description, SqlDbType.VarChar));
                    SqlParameter outputParam = DBParameterHelper.OutputParameter("@amenityID", SqlDbType.Int, 8);
                    sqlCommand.Parameters.Add(outputParam);
                    dbConnect.DoUpdate(sqlCommand);
                }
            }


		}
    }
}
