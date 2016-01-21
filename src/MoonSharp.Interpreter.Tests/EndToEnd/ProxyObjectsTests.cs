﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;

namespace MoonSharp.Interpreter.Tests.EndToEnd
{
	[TestFixture]
	public class ProxyObjectsTests
	{
		private class Proxy
		{
			[MoonSharpVisible(false)]
			public Random random;

			[MoonSharpVisible(false)]
			public Proxy(Random r)
			{
				random = r;
			}

			public int GetValue() { return 3; }
		}

		[Test]
		public void ProxyTest()
		{
			Script.GlobalOptions.CustomConverters.RegisterProxy<Proxy, Random>(
				r => new Proxy(r), p => p.random);

			UserData.RegisterType<Proxy>();

			Script S = new Script();

			S.Globals["R"] = new Random();
			S.Globals["func"] = (Action<Random>)(r => { Assert.IsNotNull(r); Assert.IsInstanceOf(typeof(Random), r); });

			S.DoString(@"
				x = R.GetValue();
				func(R);
			");

			Assert.AreEqual(3.0, S.Globals.Get("x").Number);
		}


	}
}
