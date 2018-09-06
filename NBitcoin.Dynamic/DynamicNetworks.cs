using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using System.Linq;
using System;
using System.Net;
using System.Collections.Generic;

namespace NBitcoin.Dynamic
{
	public class DynamicNetworks
	{
		private static Network _mainnet;
		private static Network _testnet;
		private static object _lock = new object();

		static Tuple<byte[], int>[] pnSeed6_main = {
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2d,0x4c,0xef,0x26}, 33300),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2d,0x4d,0x56,0xf2}, 33300),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2d,0x4d,0x45,0xef}, 33300),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2d,0x20,0x5f,0xcc}, 33300),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2d,0x4c,0x4c,0x47}, 33300),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x6c,0x3d,0xd8,0xd6}, 33300)
		};
		static Tuple<byte[], int>[] pnSeed6_test = {
          //Tuple.Create(new byte[]{})
        };

		public static void Register()
		{
			if (_mainnet == null)
			{
				_mainnet = RegisterMainnet();
			}

			if (_testnet == null)
			{
				_testnet = RegisterTestnet();
			}
		}

		public static Network Mainnet
		{
			get
			{
				return _mainnet ?? RegisterMainnet();
			}
		}

		public static Network Testnet
		{
			get
			{
				return _testnet ?? RegisterTestnet();
			}
		}

		private static Network RegisterMainnet()
		{
			lock (_lock)
			{
				var builder = new NetworkBuilder();
                _mainnet = builder.SetConsensus(new Consensus()
				{
					SubsidyHalvingInterval = 2147483647, // set to maximum value for type int. dynamic does not use 
					MajorityEnforceBlockUpgrade = 750,//from chainparams.cpp
					MajorityRejectBlockOutdated = 950,//from chainparams.cpp
					MajorityWindow = 1000,//from chainparams.cpp
					BIP34Hash = new uint256(), //no bip exists for dynamic. passing empty
					PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),//from chainparams.cpp
					PowTargetTimespan = TimeSpan.FromSeconds(30 * 64), // //from chainparams.cpp 
					PowTargetSpacing = TimeSpan.FromSeconds(2 * 64), // from util.h  
					PowAllowMinDifficultyBlocks = false, //from chainparams.cpp
					PowNoRetargeting = false, //from chainparams.cpp
					RuleChangeActivationThreshold = 321, //from chainparams.cpp 
					MinerConfirmationWindow = 30, //from chainparams.cpp
					CoinbaseMaturity = 10,//from consensus.h
					HashGenesisBlock = new uint256("0x00000e140b0c3028f898431890e9dea79ae6ca537ac9362c65b45325db712de2"),//from chainparams.cpp
					GetPoWHash = GetPoWHash
				})
				.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0x1e }) // from chainparams.cpp std::vector<unsigned char>(1,30)
				.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 0xa }) // from chainparams.cpp std::vector<unsigned char>(1,10) 
				.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 0x8c }) // from chainparams.cpp  std::vector<unsigned char>(1,140)
				.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E }) //from chainparams.cpp 
				.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 }) //from chainparams.cpp 
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("dynamic"))
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("dynamic"))
				.SetMagic(0x5e617480) // from chainparams.cpp
				.SetPort(33300) //from readme.md
				.SetRPCPort(33350) //from readme.md
				.SetName("dynamic-main")
				.AddAlias("dynamic-mainnet")
				.AddDNSSeeds(new[]
				{
				new DNSSeedData("dnsseeder.io", "dyn2.dnsseeder.io"), //from chainparams.cpp
				new DNSSeedData("dnsseeder.com", "dyn2.dnsseeder.com"), //from chainparams.cpp
				new DNSSeedData("dnsseeder.host", "dyn2.dnsseeder.host"), //from chainparams.cpp
				new DNSSeedData("dnsseeder.net", "dyn2.dnsseeder.net") //from chainparams.cpp
				})
				.AddSeeds(ToSeed(pnSeed6_main))
				.SetGenesis(new Block(new BlockHeader()
				{
					BlockTime = DateTimeOffset.FromUnixTimeSeconds(1513619300), //from chainparams ln 168
					Nonce = 626614, //from chainparams.cpp ln 168 and 100
				}))
				.BuildAndRegister();

				return _mainnet;
			}
		}

		private static Network RegisterTestnet()
		{
			lock (_lock)
			{
				var builder = new NetworkBuilder();

				_testnet = builder.SetConsensus(new Consensus()
				{
					SubsidyHalvingInterval = 2147483647, // set to maximum value for type int. dynamic does not use 
					MajorityEnforceBlockUpgrade = 510, //from chainparams.cpp
					MajorityRejectBlockOutdated = 750, //from chainparams.cpp
					MajorityWindow = 1000, //from chainparams.cpp
					PowLimit = new Target(new uint256("0000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")), //from chainparams.cpp
					PowTargetTimespan = TimeSpan.FromSeconds(30 * 64), //from chainparams.cpp
					PowTargetSpacing = TimeSpan.FromSeconds(2 * 64), // from util.h  
					PowAllowMinDifficultyBlocks = true, //from chainparams.cpp
					PowNoRetargeting = false, //from chainparams.cpp
					RuleChangeActivationThreshold = 254, //from chainparams.cpp
					MinerConfirmationWindow = 30, // from chainparams.cpp
					 CoinbaseMaturity = 10, //from consensus.h
					HashGenesisBlock = new uint256("0x000ab751d858e116043e741d097311f2382e600c219483cfda8f25c7f369cc2c"), //from chainparams.cpp
					GetPoWHash = GetPoWHash
				})
				.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0x1e }) // from chainparams.cpp std::vector<unsigned char>(1,30)
				.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 0xa }) // from chainparams.cpp std::vector<unsigned char>(1,10)
				.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 0x9e }) // from chainparams.cpp  std::vector<unsigned char>(1,158)
				.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF }) //from chainparams.cpp 
				.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 }) //from chainparams.cpp 
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tdynamic"))
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tdynamic"))
				.SetMagic(0x2f321540) //from chainparams.cpp 
				.SetPort(33300 + 100) //from chainparams.cpp 
				.SetRPCPort(33450) // from chainparamsbase.cpp
				.SetName("dynamic-test")
				.AddAlias("dynamic-testnet")
				.AddDNSSeeds(new[]
				{
				new DNSSeedData("",  ""),
				new DNSSeedData("", "")
				})
				.AddSeeds(ToSeed(pnSeed6_test))
				.SetGenesis(new Block(new BlockHeader()
				{
					BlockTime = DateTimeOffset.FromUnixTimeSeconds(1513619864), //from chainparams.cpp ln 276
					Nonce = 43629, //from chainparams.cpp ln 276
				}))
				.BuildAndRegister();

				return _testnet;
			}
		}

		private static uint256 GetPoWHash(BlockHeader header)
		{
			var headerBytes = header.ToBytes();
			var h = Crypto.Argon2d.Dynamic.Hash(headerBytes);
			return new uint256(h);
		}

		private static IEnumerable<NetworkAddress> ToSeed(Tuple<byte[], int>[] tuples)
		{
			return tuples
				.Select(t => new NetworkAddress(new IPAddress(t.Item1), t.Item2))
				.ToArray();
		}
	}
}
