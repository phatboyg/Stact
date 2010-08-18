namespace Magnum.Specs.Collections
{
	using System;


	static class RandomExtensions
	{
		static readonly char[] LowerCaseChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
		static readonly char[] UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
		static int _maxUpperCaseIndex;
		static int _maxLowerCaseIndex;

		static RandomExtensions()
		{
			_maxUpperCaseIndex = UpperCaseChars.Length - 1;
			_maxLowerCaseIndex = LowerCaseChars.Length - 1;
		}

		public static string GenerateRandomName(this Random random)
		{
			return GenerateRandomName(random, 25);
		}

		public static string GenerateRandomName(this Random random, int length)
		{
			string firstName = random.GenerateRandomNameString((length - 2) / 3 * 2);
			string lastName = random.GenerateRandomNameString((length - 2)/3);

			return lastName + ", " + firstName;
		}

		static string GenerateRandomNameString(this Random random, int length)
		{
			var chars = new char[length];
			int i = 0;
			for (; i < 1 && i < length; i++)
			{
				chars[i] = UpperCaseChars[random.Next(0, _maxUpperCaseIndex)];
			}
			for (; i < length; i++)
			{
				chars[i] = LowerCaseChars[random.Next(0, _maxLowerCaseIndex)];
			}

			return new string(chars);
		}
	}
}