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
					SubsidyHalvingInterval = 210240, //needs review. can not find in dynamic repo
					MajorityEnforceBlockUpgrade = 750,//from chainparams.cpp
					MajorityRejectBlockOutdated = 950,//from chainparams.cpp
					MajorityWindow = 1000,//from chainparams.cpp
					BIP34Hash = new uint256("0x000007d91d1254d60e2dd1ae580383070a4ddffa4c64c2eeb4a2f9ecc0414343"), //needs review. can not find in dynamic repo
					PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),//from chainparams.cpp
					PowTargetTimespan = TimeSpan.FromSeconds(30 * 64), // //from chainparams.cpp 
					PowTargetSpacing = TimeSpan.FromSeconds(2.5 * 60), // from chainparams.cpp  
					PowAllowMinDifficultyBlocks = false, //from chainparams.cpp
					PowNoRetargeting = false, //from chainparams.cpp
					RuleChangeActivationThreshold = 321, //from chainparams.cpp 
					MinerConfirmationWindow = 30, //from chainparams.cpp
					CoinbaseMaturity = 10,//from consensus.h
					HashGenesisBlock = new uint256("0x00000e140b0c3028f898431890e9dea79ae6ca537ac9362c65b45325db712de2"),//from chainparams.cpp
					GetPoWHash = GetPoWHash
				})
				.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0x44 }) //needs review. dynamic chainsparams.cpp has "Dynamic Address start with 'D'. NBitcoin NetworkBuilder class treats this field as prefix. So, set to "D" using hex 0x44 
				.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 0x05 }) //needs review. dynamic chainsparams.cpp has "Dynamic script addresses start with '5'. NBitcoin NetworkBuilder class treats this field as prefix. So, set to "5" using hex 0x05 
				.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 0x79 }) //needs review. dynamic chainsparams.cpp has "Dynamic private keys start with 'y'. NBitcoin NetworkBuilder class treats this field as prefix. So, set to "y" using hex 0x79 
				.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E }) //from chainparams.cpp 
				.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 }) //from chainparams.cpp 
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("dynamic")) //needs review. can not find in dynamic repo
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("dynamic")) //needs review. can not find in dynamic repo
				.SetMagic(0xdbb6c0fb) //needs review. readme.md has "Magic Bytes: 0x6e 0x71 0x84 0x90". Is this same parameter?
				.SetPort(33300) //from readme.md
				.SetRPCPort(33350) //from readme.md
				.SetName("dynamic-main") //needs review. not sure about this
				.AddAlias("dynamic-mainnet") //needs review. not sure about this
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
					BlockTime = DateTimeOffset.FromUnixTimeSeconds(1390095618), //needs review. does this need to be modified?
					Nonce = 28917698, //needs review. does this need to be modified?
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
					SubsidyHalvingInterval = 210240,
					MajorityEnforceBlockUpgrade = 51,
					MajorityRejectBlockOutdated = 75,
					MajorityWindow = 100,
					PowLimit = new Target(new uint256("00000fffff000000000000000000000000000000000000000000000000000000")),
					PowTargetTimespan = TimeSpan.FromSeconds(24 * 60 * 60),
					PowTargetSpacing = TimeSpan.FromSeconds(2.5 * 60),
					PowAllowMinDifficultyBlocks = true,
					PowNoRetargeting = false,
					RuleChangeActivationThreshold = 1512,
					MinerConfirmationWindow = 2016,
					CoinbaseMaturity = 100,
					HashGenesisBlock = new uint256("0x00000bafbc94add76cb75e2ec92894837288a481e5c005f6563d91623bf8bc2c"),
					GetPoWHash = GetPoWHash
				})
				.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0x8C })
				.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 0x13 })
				.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 0xEF })
				.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
				.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tdynamic"))
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tdynamic"))
				.SetMagic(0xf1c8d2fd)
				.SetPort(19999)
				.SetRPCPort(19998)
				.SetName("dynamic-test")
				.AddAlias("dynamic-testnet")
				.AddDNSSeeds(new[]
				{
				new DNSSeedData("dynamicdot.io",  "testnet-seed.dynamicdot.io"),
				new DNSSeedData("masternode.io", "test.dnsseed.masternode.io")
				})
				.AddSeeds(ToSeed(pnSeed6_test))
				.SetGenesis(new Block(new BlockHeader()
				{
					BlockTime = DateTimeOffset.FromUnixTimeSeconds(1390666206),
					Nonce = 3861367235,
				}))
				.BuildAndRegister();

				return _testnet;
			}
		}

		private static uint256 GetPoWHash(BlockHeader header)
		{
			var headerBytes = header.ToBytes();
			var h = Crypto.SCrypt.ComputeDerivedKey(headerBytes, headerBytes, 1024, 1, 1, null, 32);
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
