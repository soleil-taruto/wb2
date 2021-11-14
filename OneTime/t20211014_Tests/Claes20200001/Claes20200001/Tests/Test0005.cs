using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0005
	{
		public void Test01()
		{
			Test01_a(null);
			Test01_a(new string[] { });
			Test01_a(new string[] { null });
			Test01_a(new string[] { null, null });
			Test01_a(new string[] { null, null, null });
			Test01_a(new string[] { "" });
			Test01_a(new string[] { "", "" });
			Test01_a(new string[] { "", "", "" });
			Test01_a(new string[] { "ABC", });
			Test01_a(new string[] { "ABC", "DEF" });
			Test01_a(new string[] { "ABC", "DEF", "GHI" });
			Test01_a(new string[] { null, "xxxxx" });
			Test01_a(new string[] { "xxxxx", null });
			Test01_a(new string[] { null, "123", "456" });
			Test01_a(new string[] { "123", null, "456" });
			Test01_a(new string[] { "123", "456", null });

			Console.WriteLine("OK!");
		}

		private void Test01_a(string[] strs)
		{
			string seriStr = SerializeStrings(strs);
			string[] strs2 = DeserializeStrings(seriStr);

			if (string.IsNullOrEmpty(seriStr))
				throw null;

			if (strs == null)
			{
				if (strs2 != null)
					throw null;
			}
			else
			{
				if (strs2 == null)
					throw null;

				if (SCommon.Comp(
					strs,
					strs2,
					(a, b) =>
					{
						if (a == null && b == null)
							return 0;

						if (a == null) // ? (a, b) == (null, not null) --> a < b
							return -1;

						if (b == null) // ? (a, b) == (not null, null) --> a > b
							return 1;

						return SCommon.Comp(a, b);
					}
					) != 0
					)
					throw null;
			}
		}

		private string SerializeStrings(string[] strs)
		{
			const char DUMMY_CHAR = '?';

			if (strs == null)
				strs = new string[] { "" };
			else
				strs = new string[] { "", "" }.Concat(strs).ToArray();

			strs = strs
				.Select(str => str == null ? "" : DUMMY_CHAR + str)
				.ToArray();

			byte[][] bStrs = strs
				.Select(str => Encoding.UTF8.GetBytes(str))
				.ToArray();

			StringBuilder buff = new StringBuilder();

			foreach (byte[] bStr in bStrs)
			{
				buff.Append(((uint)bStr.Length).ToString("x8"));

				foreach (byte bChr in bStr)
					buff.Append(bChr.ToString("x2"));
			}
			return buff.ToString();
		}

		private string[] DeserializeStrings(string seriStr)
		{
			List<byte[]> bStrs = new List<byte[]>();

			for (int index = 0; index < seriStr.Length; )
			{
				int size = (int)Convert.ToUInt32(seriStr.Substring(index, 8), 16);
				index += 8;

				byte[] bStr = new byte[size];

				for (int i = 0; i < size; i++)
				{
					byte bChr = (byte)Convert.ToUInt32(seriStr.Substring(index, 2), 16);
					index += 2;

					bStr[i] = bChr;
				}
				bStrs.Add(bStr);
			}
			string[] strs = bStrs
				.Select(bStr => Encoding.UTF8.GetString(bStr))
				.ToArray();

			strs = strs
				.Select(str => str.Length == 0 ? null : str.Substring(1))
				.ToArray();

			if (strs.Length < 2)
				strs = null;
			else
				strs = strs.Skip(2).ToArray();

			return strs;
		}
	}
}

#if false

import java.util.*;
import java.nio.charset.*;

public class Main {
    public static void main(String[] args) throws Exception {
        
		test01_a(null);
		test01_a(new String[] { });
		test01_a(new String[] { null });
		test01_a(new String[] { null, null });
		test01_a(new String[] { null, null, null });
		test01_a(new String[] { "" });
		test01_a(new String[] { "", "" });
		test01_a(new String[] { "", "", "" });
		test01_a(new String[] { "ABC", });
		test01_a(new String[] { "ABC", "DEF" });
		test01_a(new String[] { "ABC", "DEF", "GHI" });
		test01_a(new String[] { null, "xxxxx" });
		test01_a(new String[] { "xxxxx", null });
		test01_a(new String[] { null, "123", "456" });
		test01_a(new String[] { "123", null, "456" });
		test01_a(new String[] { "123", "456", null });
		
		System.out.println("OK!");
    }
    
    private static void test01_a(String[] strs) throws Exception {
        
		String seriStr = serializeStrings(strs == null ? null : Arrays.asList(strs));
		List<String> strList2 = deserializeStrings(seriStr);
		String[] strs2 = strList2 == null ? null : strList2.toArray(new String[0]);

		if (seriStr == null || seriStr.length() == 0) {
			throw null;
		}

		if (strs == null) {
			if (strs2 != null) {
				throw null;
			}
		} else {
			if (strs2 == null) {
				throw null;
			}
			
			for (int index = 0; index < strs.length; index++) {
			    String str = strs[index];
			    String str2 = strs2[index];
			    
			    if (str == null) {
                    if (str2 != null) {
                        throw null;
                    }
			    } else {
                    if (str2 == null) {
                        throw null;
                    }
                    
                    if (!str.equals(str2)) {
                        throw null;
                    }
			    }
			}
		}
    }
    
	private static String serializeStrings(List<String> strs) throws Exception {
	    
		List<String> strs2 = new ArrayList<String>();
		List<byte[]> bStrs = new ArrayList<byte[]>();
		StringBuffer buff = new StringBuffer();

		if (strs == null) {
		    strs2.add("");
		} else {
		    strs2.add("");
		    strs2.add("");
		    strs2.addAll(strs);
		}
		
		for (String f_str : strs2) {
		    String str = f_str;
		    
		    if (str == null) {
		        str = "";
		    } else {
		        str = '?' + str;
		    }
		    
		    byte[] bStr = str.getBytes("UTF-8");
		    
		    bStrs.add(bStr);
		}

		for (byte[] bStr : bStrs) {
			buff.append(String.format("%08x", bStr.length));

			for (byte bChr : bStr) {
    			buff.append(String.format("%02x", bChr & 0xff));
			}
		}
		return buff.toString();
	}

	private static List<String> deserializeStrings(String seriStr) {

		List<byte[]> bStrs = new ArrayList<byte[]>();
		List<String> strs = new ArrayList<String>();

		for (int index = 0; index < seriStr.length(); ) {
			int size = Integer.parseInt(seriStr.substring(index, index + 8), 16);
			index += 8;

			byte[] bStr = new byte[size];

			for (int i = 0; i < size; i++) {
				byte bChr = (byte)Integer.parseInt(seriStr.substring(index, index + 2), 16);
				index += 2;

				bStr[i] = bChr;
			}
			bStrs.add(bStr);
		}
		
		for (byte[] bStr : bStrs) {
		    String str = new String(bStr, Charset.forName("UTF-8"));
		    
		    if (str.length() == 0) {
		        str = null;
		    } else {
		        str = str.substring(1);
		    }
		    
		    strs.add(str);
		}
		
		if (strs.size() < 2) {
		    strs = null;
		} else {
		    strs = strs.subList(2, strs.size());
		}
		return strs;
	}
}

#endif
