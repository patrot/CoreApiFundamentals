using System;
using CoreCodeCamp.Data;

namespace CoreCodeCamp.Models
{
  public class CampModelToCampMapper
  {
    public CampModelToCampMapper()
    {
    }

    public Camp Map(CampModel campModel)
    {
      Location location = new Location {
        Address1 = campModel.LocationAddress1,
        Address2 = campModel.LocationAddress2,
        Address3 = campModel.LocationAddress3,
        CityTown = campModel.LocationCityTown,
        PostalCode = campModel.LocationPostalCode,
        Country = campModel.LocationCountry,
        StateProvince = campModel.LocationStateProvince
      };

      Camp camp = new Camp {
        Name = campModel.Name,
        Moniker = campModel.Moniker,
        EventDate = campModel.EventDate,
        Length = campModel.Length,
        Location = location,
        //Talks = campModel.Talks
      };

      return camp;
    }
  }
}
