using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Config
{
    public class Categories
    {

        public static IEnumerable<Category> GetAll()
        {
            return new List<Category>
                {
                    new Category("Amphibians", "Amphibians", new [] { "Animalia: Chordata: Amphibia" }),
                    new Category("Birds", "Birds", new [] { "Animalia: Chordata: Aves" }),
                    new Category("Fishes", "Fishes", new [] { "Animalia: Chordata" }),
                    new Category("Fungi & Lichens", "Fungi & Lichens", new [] { "Fungi" }),
                    new Category("Invertebrates", "Invertebrates", new [] { "Animalia" }),
                    new Category("Mammals", "Mammals", new [] { "Animalia: Chordata: Mammalia" }),
                    new Category("Others", "Others", new string[] {  }),
                    new Category("Plants", "Plants", new [] { "Plantae" }),
                    new Category("Reptiles", "Reptiles", new [] { "Animalia: Chordata: Reptilia" })
                };
        }

        public static object GetSelectList(params string[] categories)
        {
            return GetAll()
                .Select(x => new
                    {
                        Text = x.Name,
                        Value = x.Id,
                        Selected = categories.Any(y => x.Id.ToLower() == y.ToLower().Trim())
                    });
        }

        public static bool IsValidCategory(string category)
        {
            return GetAll().Any(x => x.Id.ToLower() == category.ToLower().Trim());
        }

        public static object Get(params string[] categories)
        {
            return GetAll().Where(x => categories.Any(y => y.ToLower() == x.Id.ToLower()));
        }

    }
}
