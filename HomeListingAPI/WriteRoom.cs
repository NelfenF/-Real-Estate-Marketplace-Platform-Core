﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeListingAPI
{
    internal static class WriteRoom
    {
        internal static int CreateNew(int homeID, Room room)
        {
            DBConnect dbConnect = new DBConnect();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "P4_CreateNewRoom";

            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@homeID", (int)homeID, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@roomType", room.Type.ToString(), SqlDbType.VarChar, 50));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@height", room.Height, SqlDbType.Int, 8));
            sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@width", room.Width, SqlDbType.Int, 8));

            SqlParameter outputParam = DBParameterHelper.OutputParameter("@roomID", SqlDbType.Int, 8);
            sqlCommand.Parameters.Add(outputParam);

            dbConnect.DoUpdate(sqlCommand);
            return (int)outputParam.Value;
        }

        internal static void DeleteRoom(int homeID)
        {
			try
			{
				DBConnect dbConnect = new DBConnect();
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "P4_DeleteRoom";
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

        internal static void UpdateRooms(int homeID, Rooms updatedRooms, Rooms oldRooms)
        {

			// Remove deleted rooms
			foreach (Room currentOldRoom in oldRooms.List)
			{
				// Check if the current old room exists in the updated rooms list
				bool existsInUpdatedList = updatedRooms.List.Any(updatedRoom => updatedRoom.RoomID == currentOldRoom.RoomID);

				if (!existsInUpdatedList)
				{
					// Delete the old room because it wasn't present in the updated rooms
					DBConnect dbConnect = new DBConnect();
					SqlCommand sqlCommand = new SqlCommand
					{
						CommandType = CommandType.StoredProcedure,
						CommandText = "P4_RemoveRoom"
					};
					sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@RoomID", (int)currentOldRoom.RoomID, SqlDbType.Int, 8));
					dbConnect.DoUpdate(sqlCommand);
				}
			}
			//Update or add rooms
			foreach (Room currentRoom in updatedRooms.List)
            {
                if (currentRoom.RoomID != 0)
                {
                    DBConnect dbConnect = new DBConnect();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "P4_UpdateRoom";
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@RoomID", (int)currentRoom.RoomID, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@HomeID", homeID, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@RoomType", currentRoom.Type.ToString(), SqlDbType.VarChar, 50));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@Height", currentRoom.Height, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@Width", currentRoom.Width, SqlDbType.Int, 8));
                    dbConnect.DoUpdate(sqlCommand);
                }
                else
                {
                    //Create new rooms
                    DBConnect dbConnect = new DBConnect();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "P4_CreateNewRoom";
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@homeID", (int)homeID, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<string>("@roomType", currentRoom.Type.ToString(), SqlDbType.VarChar, 50));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@height", currentRoom.Height, SqlDbType.Int, 8));
                    sqlCommand.Parameters.Add(DBParameterHelper.InputParameter<int>("@width", currentRoom.Width, SqlDbType.Int, 8));
                    SqlParameter outputParam = DBParameterHelper.OutputParameter("@roomID", SqlDbType.Int, 8);
                    sqlCommand.Parameters.Add(outputParam);
                    dbConnect.DoUpdate(sqlCommand);
                }
            }


		}
    }
}
