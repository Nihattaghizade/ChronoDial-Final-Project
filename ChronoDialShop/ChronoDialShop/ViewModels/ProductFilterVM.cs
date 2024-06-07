using ChronoDialShop.Models;
using ChronoDialShop.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChronoDialShop.ViewModels
{
	public class ProductFilterVM
	{
		public List<int> BrandsIds { get; set; }
		public List<int> VendorsIds { get; set; }
		public List<int> BandTypesIds { get; set; }
		public List<int> VisualizationsIds { get; set; }
		public List<int> InnerColorsIds { get; set; }
		//public int MinPrice { get; set; } = 0;
		//public int MaxPrice { get; set; } = 20000;
		//public Gender Gender { get; set; }

	}
}

