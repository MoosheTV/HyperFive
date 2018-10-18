using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using HyperFive.Util;

namespace HyperFive.Game
{
	public class GameController : ClientAccessor
	{
		private readonly List<HudComponent> _hiddenComponents = new List<HudComponent>();

		public GameController( Client client ) : base( client ) {
			Client.RegisterTickHandler( OnTick );
		}

		/// <summary>
		/// Hides the component given from being rendered.
		/// </summary>
		/// <param name="comp">The component to hide.</param>
		public void HideComponent( HudComponent comp ) {
			if( !_hiddenComponents.Contains( comp ) ) {
				_hiddenComponents.Add( comp );
			}
		}

		/// <summary>
		/// Unhides the component, and will render it again.
		/// </summary>
		/// <param name="comp">The component to stop hiding.</param>
		public void ShowComponent( HudComponent comp ) {
			if( _hiddenComponents.Contains( comp ) ) {
				_hiddenComponents.Remove( comp );
			}
		}


		private async Task OnTick() {
			try {
				// Hide HUD components
				if( _hiddenComponents.Any() ) {
					foreach( var comp in new List<HudComponent>( _hiddenComponents ) ) {
						Screen.Hud.HideComponentThisFrame( comp );
					}
				}
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}
	}
}
