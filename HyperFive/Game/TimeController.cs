using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using HyperFive.Util;

namespace HyperFive.Game
{
	public class TimeController : ClientAccessor
	{
		/// <summary>
		/// The Current Day in-game.
		/// </summary>
		public DateTime Day
		{
			get {
				var day = Function.Call<int>( Hash.GET_CLOCK_DAY_OF_MONTH );
				var month = Function.Call<int>( Hash.GET_CLOCK_MONTH );
				var year = Function.Call<int>( Hash.GET_CLOCK_YEAR );
				return new DateTime( year, month, day );
			}
			set => Function.Call( Hash.SET_CLOCK_DATE, value.Day, value.Month, value.Year );
		}

		public int Hour
		{
			get => Function.Call<int>( Hash.GET_CLOCK_HOURS );
			set => Function.Call( Hash.NETWORK_OVERRIDE_CLOCK_TIME, value, Minute, Second );
		}

		public int Minute
		{
			get => Function.Call<int>( Hash.GET_CLOCK_MINUTES );
			set => Function.Call( Hash.NETWORK_OVERRIDE_CLOCK_TIME, Hour, value, Second );
		}

		public int Second
		{
			get => Function.Call<int>( Hash.GET_CLOCK_SECONDS );
			set => Function.Call( Hash.NETWORK_OVERRIDE_CLOCK_TIME, Hour, Minute, value );
		}

		public TimeController( Client client ) : base( client ) {

		}

		private void OnTick() {
			try {

			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}
	}
}
