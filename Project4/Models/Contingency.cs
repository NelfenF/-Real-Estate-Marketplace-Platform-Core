﻿namespace Project4.Models
{
    [Serializable]
    public class Contingency : ICloneable<Contingency>
    {
        private int contingencyID;
        private int offerID;
        private string contingency;

        public int ContingencyID
        {
            get { return contingencyID; }
            set { contingencyID = value; }
        }

        public int OfferID
        {
            get { return offerID; }
            set { offerID = value; }
        }

        public string OfferContingency
        {
            get { return contingency; }
            set { contingency = value; }
        }

        public Contingency(int id, int offerID, string contingency)
        {
            ContingencyID = id;
            OfferID = offerID;
            OfferContingency = contingency;
        }

        public Contingency(int offerID, string contingency)
        {
            OfferID = offerID;
            OfferContingency = contingency;
        }

        public Contingency()
        {

        }

        public Contingency Clone()
        {
            return new Contingency(ContingencyID, OfferID, OfferContingency);
        }

    }
}
