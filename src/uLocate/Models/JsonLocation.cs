﻿namespace uLocate.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Helpers;

    using uLocate.Persistance;

    public class JsonLocation
    {
        public Guid Key { get; set; }
        public Guid LocationTypeKey { get; set; }
        public string LocationTypeName { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        //public List<JsonPropertyData> AllPropertyData { get; set; }
        public List<JsonPropertyData> CustomPropertyData { get; set; }

        public JsonLocation()
        {
            //this.AllPropertyData = new List<JsonPropertyData>();
            this.CustomPropertyData = new List<JsonPropertyData>();
        }

        public JsonLocation(Location ConvertedFromLocation)
        {
            //Basic Location properties 
            this.Key = ConvertedFromLocation.Key;
            this.Name = ConvertedFromLocation.Name;
            this.LocationTypeKey = ConvertedFromLocation.LocationTypeKey;
            this.LocationTypeName = ConvertedFromLocation.LocationType.Name;

            this.Latitude = ConvertedFromLocation.Latitude;
            this.Longitude = ConvertedFromLocation.Longitude;

            //Address
            this.Address1 = ConvertedFromLocation.Address.Address1;
            this.Address2 = ConvertedFromLocation.Address.Address2;
            this.Locality = ConvertedFromLocation.Address.Locality;
            this.Region = ConvertedFromLocation.Address.Region;
            this.PostalCode = ConvertedFromLocation.Address.PostalCode;
            this.CountryCode = ConvertedFromLocation.Address.CountryCode;

            this.CustomPropertyData = new List<JsonPropertyData>();
            foreach (var Prop in ConvertedFromLocation.PropertyData)
            {
                //Check for special props
                switch (Prop.PropertyAlias)
                {
                    case Constants.DefaultLocPropertyAlias.Phone:
                        this.Phone = Prop.Value.ToString();
                        break;
                    case Constants.DefaultLocPropertyAlias.Email:
                        this.Email = Prop.Value.ToString();
                        break;
                    case Constants.DefaultLocPropertyAlias.Address1:
                        break;
                    case Constants.DefaultLocPropertyAlias.Address2:
                        break;
                    case Constants.DefaultLocPropertyAlias.Locality:
                        break;
                    case Constants.DefaultLocPropertyAlias.Region:
                        break;
                    case Constants.DefaultLocPropertyAlias.PostalCode:
                        break;
                    case Constants.DefaultLocPropertyAlias.CountryCode:
                        break;
                    default:
                        this.CustomPropertyData.Add(new JsonPropertyData(Prop));
                        //this.AllPropertyData.Add(new JsonPropertyData(Prop));
                        break;
                }
            }
        }

        public Location ConvertToLocation()
        {
            Location Entity;

            if (this.Key != Guid.Empty)
            {
                //Lookup existing entity
                Entity = Repositories.LocationRepo.GetByKey(this.Key);

                //Update Location properties as needed
                Entity.Name = this.Name;
                Entity.LocationTypeKey = this.LocationTypeKey;

            }
            else
            {
                //Create new entity
                Entity = new Location(Name = this.Name, LocationTypeKey = this.LocationTypeKey);
            }

            //Update lat/long
            Entity.Latitude = this.Latitude;
            Entity.Longitude = this.Longitude;

            //Add Address properties
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.Address1, this.Address1);
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.Address2, this.Address2);
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.Locality, this.Locality);
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.Region, this.Region);
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.PostalCode, this.PostalCode);
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.CountryCode, this.CountryCode);

            //Deal with Properties    
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.Phone, this.Phone);
            Entity.AddPropertyData(Constants.DefaultLocPropertyAlias.Email, this.Email);

            //Add custom properties
            foreach (var JsonProp in this.CustomPropertyData)
            {
                Entity.AddPropertyData(JsonProp.PropAlias, JsonProp.PropData);
            }

            return Entity;
        }

    }

    public class JsonPropertyData
    {
        public Guid Key { get; set; }
        public string PropAlias { get; set; }
        public object PropData { get; set; }

        public JsonPropertyData(LocationPropertyData Prop)
        {
            if (Prop != null)
            {
                this.Key = Prop.Key;
                this.PropAlias = Prop.PropertyAlias;
                this.PropData = Prop.Value.ValueObject;
            }
        }

        internal JsonPropertyData()
        {
        }
    }
}
