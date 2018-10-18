using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace HyperFive.Game
{
	public class DecorController : ClientAccessor
	{
		private readonly List<Decor> _decors = new List<Decor>();

		public DecorController( Client client ) : base( client ) {

		}

		public float GetFloat( Entity entity, string property ) {
			return Function.Call<float>( Hash._DECOR_GET_FLOAT, entity.Handle, property );
		}

		public int GetInt( Entity entity, string property ) {
			return Function.Call<int>( Hash.DECOR_GET_INT, entity.Handle, property );
		}

		public bool GetBool( Entity entity, string property ) {
			return Function.Call<bool>( Hash.DECOR_GET_BOOL, entity.Handle, property );
		}

		/// <summary>
		/// Attempts to set an integer decoration on an entity, given its handle, property, 
		/// and value. If the decor isn't registered, it will attempt to register it before
		/// attempting to set the value.
		/// </summary>
		/// <param name="entity">The entity to set the value to</param>
		/// <param name="property">The property name</param>
		/// <param name="value">The value to set</param>
		public void SetInt( Entity entity, string property, int value ) {
			RegisterDecor( property, DecorType.Int );
			Function.Call( Hash.DECOR_SET_INT, entity.Handle, property, value );
		}

		/// <summary>
		/// Attempts to set a float decoration on an entity, given its handle, property, 
		/// and value. If the decor isn't registered, it will attempt to register it before
		/// attempting to set the value.
		/// </summary>
		/// <param name="entity">The entity to set the value to</param>
		/// <param name="property">The property name</param>
		/// <param name="value">The value to set</param>
		public void SetFloat( Entity entity, string property, int value ) {
			RegisterDecor( property, DecorType.Float );
			Function.Call( Hash._DECOR_SET_FLOAT, entity.Handle, property, value );
		}

		/// <summary>
		/// Attempts to set a boolean decoration on an entity, given its handle, property, 
		/// and value. If the decor isn't registered, it will attempt to register it before
		/// attempting to set the value.
		/// </summary>
		/// <param name="entity">The entity to set the value to</param>
		/// <param name="property">The property name</param>
		/// <param name="value">The value to set</param>
		public void SetBool( Entity entity, string property, int value ) {
			RegisterDecor( property, DecorType.Bool );
			Function.Call( Hash.DECOR_SET_BOOL, entity.Handle, property, value );
		}

		/// <summary>
		/// Registers a decor if one does not exist already.
		/// </summary>
		/// <param name="propertyName">The property name to register</param>
		/// <param name="type">The type to register</param>
		public void RegisterDecor( string propertyName, DecorType type ) {
			RegisterDecor( new Decor( propertyName, type ) );
		}

		public Decor GetRegisteredDecor( string propertyName, DecorType type ) {
			return _decors.FirstOrDefault( p => p.Type == type && p.PropertyName == propertyName );
		}

		/// <summary>
		/// Registers a decor if one does not exist already.
		/// </summary>
		/// <param name="decor">The decor to register</param>
		public void RegisterDecor( Decor decor ) {
			if( _decors.Any( d => d.Type == decor.Type && d.PropertyName == decor.PropertyName ) ) return;
			_decors.Add( decor );
			Function.Call( Hash.DECOR_REGISTER, decor.PropertyName, decor.Type );
		}
	}

	public sealed class Decor
	{
		public string PropertyName { get; }
		public DecorType Type { get; }

		public Decor( string propertyName, DecorType type ) {
			PropertyName = propertyName;
			Type = type;
		}
	}

	public enum DecorType
	{
		Float = 1,
		Bool,
		Int
	}
}
