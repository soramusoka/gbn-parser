﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetByNameLibrary.Interfaces
{
	public interface IReplacer
	{
		String DelBadChar(ref String str);
		String DelWithRegex(String str);
	}
}
