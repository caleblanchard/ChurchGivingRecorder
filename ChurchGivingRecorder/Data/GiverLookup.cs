using ChurchGivingRecorder.Models;
using NonFactors.Mvc.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Data
{
    public class GiverLookup : MvcLookup<Giver>
    {
        private ApplicationDbContext Context { get; }

        public GiverLookup(ApplicationDbContext context)
        {
            Context = context;

            Columns = new List<LookupColumn>
            {
                new LookupColumn("EnvelopIdString", "Envelope Id") { Filterable = true },
                new LookupColumn("Name", "Giver Name") { Filterable = true }
            };

            GetId = (model) => model.Id.ToString();
            GetLabel = (model) => model.EnvelopeNameDisplay;
        }

        public GiverLookup()
        {
            Url = "/Givers/AllGivers";
            Title = "Givers";

            Columns = new List<LookupColumn>
            {
                new LookupColumn("EnvelopIdString", "Envelope Id") { Filterable = true },
                new LookupColumn("Name", "Giver Name") { Filterable = true }
            };

            Filter.Sort = "EnvelopeID";
            Filter.Order = LookupSortOrder.Asc;
        }

        public override IQueryable<Giver> GetModels()
        {
            return Context.Set<Giver>();
        }
    }
}
