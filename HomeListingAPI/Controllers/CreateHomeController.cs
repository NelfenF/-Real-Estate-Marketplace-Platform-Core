﻿using Microsoft.AspNetCore.Mvc;

namespace HomeListingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateHomeController : Controller
    {
        //Http Post Create new Home / return a home id int 
        [HttpPost("CreateHomeListing")]
        public string Post([FromBody]Home home)
        {
            try
            {

                int homeID = WriteHome.CreateNew(home);
                if (homeID > -1)
                {
                    home.HomeID = homeID;
                    foreach (Image image in home.Images.List)
                    {
                        int imageID = WriteHomeImage.CreateNew(homeID, image);
                        if (imageID < 0)
                        {
                            WriteHome.DeleteHome(homeID);
                            return "-2";
                        }
                    }
                    foreach (Room room in home.Rooms.List)
                    {
                        int roomID = WriteRoom.CreateNew(homeID, room);
                        if (roomID < 0)
                        {
                            WriteHome.DeleteHome(homeID);
                            return "-3";
                        }
                    }
                    foreach (Utility utility in home.Utilities.List)
                    {
                        int utilityID = WriteUtility.CreateNew(homeID, utility);
                        if (utilityID < 0)
                        {
                            WriteHome.DeleteHome(homeID);
                            return "-4";
                        }
                    }
                    foreach (Amenity amenity in home.Amenities.List)
                    {
                        int amenityID = WriteAmenity.CreateNew(homeID, amenity);
                        if (amenityID < 0)
                        {
                            WriteHome.DeleteHome(homeID);
                            return "-5";
                        }
                    }
                    int temperatureControlID = WriteTemperatureControl.CreateNew(homeID, home.TemperatureControl);
                    if (temperatureControlID < 0)
                    {
                        WriteHome.DeleteHome(homeID);
                        return "-6";
                    }
                } 
                return homeID.ToString();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            }
    }
}
