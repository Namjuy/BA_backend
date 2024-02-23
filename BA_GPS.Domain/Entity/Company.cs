using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BA_GPS.Domain.Entity
{
	[Table("Companies")]
	public class Company
	{
		[Key]
		public int CompanyId { get; set; }

		[NotNull]
		public string CompanyName { get; set; }
	}
}

