using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using System.Linq;
using System;
using System.Net;
using System.Collections.Generic;

namespace NBitcoin.Dash
{
	public class DashNetworks
	{
		private static Network _mainnet;
		private static Network _testnet;
		private static object _lock = new object();

		static Tuple<byte[], int>[] pnSeed6_main = {
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xb3,0x2b,0x80,0xef}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x80,0x7f,0x6a,0xeb}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x25,0x9d,0xfa,0x0a}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xa2,0xd1,0x63,0x23}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x6c,0x3d,0xd2,0x36}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xac,0xf5,0x05,0x84}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2e,0xa2,0x42,0x0a}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x4e,0x6d,0xb2,0xc3}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x8a,0x80,0xa9,0x5e}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x34,0x0b,0x8d,0xe5}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x25,0x3b,0x15,0x3a}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x2e,0x69,0x76,0x0f}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xb2,0x21,0x7e,0xdd}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x68,0xec,0x17,0x83}, 9999),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x6c,0x3d,0xd1,0x25}, 9999)
		};

		static Tuple<byte[], int>[] pnSeed6_test = { };

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
					SubsidyHalvingInterval = 210240,
					MajorityEnforceBlockUpgrade = 750,
					MajorityRejectBlockOutdated = 950,
					MajorityWindow = 1000,
					BIP34Hash = new uint256("0x000007d91d1254d60e2dd1ae580383070a4ddffa4c64c2eeb4a2f9ecc0414343"),
					PowLimit = new Target(new uint256("00000fffff000000000000000000000000000000000000000000000000000000")),
					PowTargetTimespan = TimeSpan.FromSeconds(24 * 60 * 60),
					PowTargetSpacing = TimeSpan.FromSeconds(2.5 * 60),
					PowAllowMinDifficultyBlocks = false,
					PowNoRetargeting = false,
					RuleChangeActivationThreshold = 1916,
					MinerConfirmationWindow = 2016,
					CoinbaseMaturity = 100,
					HashGenesisBlock = new uint256("0x00000ffd590b1485b3caadc19b22e6379c733355108f107a430458cdf3407ab6"),
					GetPoWHash = GetPoWHash
				})
				.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0x4C })
				.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 0x10 })
				.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 0xCC })
				.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
				.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("dash"))
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("dash"))
				.SetMagic(0xdbb6c0fb)
				.SetPort(9999)
				.SetRPCPort(9998)
				.SetName("dash-main")
				.AddAlias("dash-mainnet")
				.AddDNSSeeds(new[]
				{
				new DNSSeedData("dash.org", "dnsseed.dash.org"),
				new DNSSeedData("dashdot.io", "dnsseed.dashdot.io"),
				new DNSSeedData("masternode.io", "dnsseed.masternode.io"),
				new DNSSeedData("dashpay.io", "dnsseed.dashpay.io")
				})
				.AddSeeds(ToSeed(pnSeed6_main))
				.SetGenesis(new Block(new BlockHeader()
				{
					BlockTime = DateTimeOffset.FromUnixTimeSeconds(1390095618),
					Nonce = 28917698,
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
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tdash"))
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tdash"))
				.SetMagic(0xf1c8d2fd)
				.SetPort(19999)
				.SetRPCPort(19998)
				.SetName("dash-test")
				.AddAlias("dash-testnet")
				.AddDNSSeeds(new[]
				{
				new DNSSeedData("dashdot.io",  "testnet-seed.dashdot.io"),
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