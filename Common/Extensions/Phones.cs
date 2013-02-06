using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class Phones
{
	public static string FormatPhone(this string number)
	{
		number = Regex.Replace(number, RegexPattern.PATHWAY_TELEPHONE_US, "$1$2$3").Trim();

		if (number.Length == 10) return Regex.Replace(number, RegexPattern.PATHWAY_TELEPHONE_US, "($1) $2-$3");
		else return number;

	}
}
