using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DemoObserver
{

	public class EventDispatcher : MonoBehaviour
	{
		#region Singleton

		static EventDispatcher s_instance;
		public static EventDispatcher Instance
		{
			get
			{
				// instance not exist, then create new one
				if (s_instance == null)
				{
					// create new Gameobject, and add EventDispatcher component
					GameObject singletonObject = new GameObject();
					s_instance = singletonObject.AddComponent<EventDispatcher>();
					singletonObject.name = "Singleton - EventDispatcher";
					Common.Log("Create singleton : {0}", singletonObject.name);
				}
				return s_instance;
			}
			private set { }
		}


		void Awake ()
		{
			// check if there's another instance already exist in scene
			if (s_instance != null && s_instance.GetInstanceID() != this.GetInstanceID())
			{
				// Destroy this instances because already exist the singleton of EventsDispatcer
				Common.Log("An instance of EventDispatcher already exist : <{1}>, So destroy this instance : <{2}>!!", s_instance.name, name);
				Destroy(gameObject);
			}
			else
			{
				// set instance
				s_instance = this as EventDispatcher;
			}
		}


		void OnDestroy ()
		{
			// reset this static var to null if it's the singleton instance
			if (s_instance == this)
				s_instance = null;
		}

		#endregion




		#region Init, main component declare

		/// Store all "listener"
		Dictionary<EventID, Action<Component, object>> _listenersDict
		= new Dictionary<EventID, Action<Component, object>>();

		#endregion



		#region Add Listeners, Post events, Remove listener

		/// <summary>
		/// Register to listen for eventID
		/// </summary>
		/// <param name="eventID">EventID that object want to listen</param>
		/// <param name="callback">Callback will be invoked when this eventID be raised</param>
		public void RegisterListener (EventID eventID, Action<Component, object> callback)
		{
			// checking params
			Common.Assert(callback != null, "AddListener, event {0}, callback = null !!", eventID.ToString());
			Common.Assert(eventID != EventID.None, "RegisterListener, event = None !!");

			// check if listener exist in distionary
			if (_listenersDict.ContainsKey(eventID))
			{
				// add callback to our collection
				_listenersDict[eventID] += callback;
			}
			else
			{
				// add new key
				_listenersDict.Add(eventID, callback);
			}
		}

		/// <summary>
		/// Posts the event. This will notify all listener that register for this event
		/// </summary>
		/// <param name="eventID">EventID.</param>
		/// <param name="sender">Sender, in some case, the Listener will need to know who send this message.</param>
		/// <param name="param">Parameter. Can be anything (struct, class ...), Listener will make a cast to get the data</param>
		public void PostEvent (EventID eventID, Component sender, object param = null)
		{
			// checking params
			Common.Assert(eventID != EventID.None, "PostNotification, event = None !!");
			Common.Assert(sender != null, "PostEvent, event {0}, sender = null !!", eventID.ToString());

			Action<Component, object> actionList;
			if (_listenersDict.TryGetValue(eventID, out actionList))
			{
				actionList(sender, param);
			}
			else
			{
				// if not exist, just warning, don't throw exceptoin
				Common.LogWarning(this, "PostNotification, event : {0}, no listener found !!", eventID.ToString());
			}
		}

		/// <summary>
		/// Removes the listener. Use to Unregister listener
		/// </summary>
		/// <param name="eventID">EventID.</param>
		/// <param name="callback">Callback.</param>
		public void RemoveListener (EventID eventID, Action<Component, object> callback)
		{
			// checking params
			Common.Assert(callback != null, "RemoveListener, event {0}, callback = null !!", eventID.ToString());
			Common.Assert(eventID != EventID.None, "AddListener, event = None !!");

			// check if exist listener in distionary, then call delegate
			if (_listenersDict.ContainsKey(eventID))
			{
				_listenersDict[eventID] -= callback;
				// if there's no listener remain for this eventID, remove it
				if (_listenersDict[eventID] == null)
				{
					_listenersDict.Remove(eventID);
				}
			}
			else
			{
				// if not exist, just warning, dont throw exceptoin
				Common.LogWarning(this, "RemoveListener, event : {0}, no listener found", eventID.ToString());
			}
		}


		/// <summary>
		/// Clears all the listener.
		/// </summary>
		public void ClearAllListener ()
		{
			_listenersDict.Clear();
		}

		#endregion
	}



	#region Extension class
	/// <summary>
	/// Delare some "shortcut" for using EventDispatcher easier
	/// </summary>
	public static class EventDispatcherExtension
	{
		/// Use for registering with EventsManager
		public static void RegisterListener (this MonoBehaviour sender, EventID eventID, Action<Component, object> callback)
		{
			EventDispatcher.Instance.RegisterListener(eventID, callback);
		}


		/// Post event with param
		public static void PostEvent (this MonoBehaviour sender, EventID eventID, object param)
		{
			EventDispatcher.Instance.PostEvent(eventID, sender, param);
		}


		/// Post event with no param (param = null)
		public static void PostEvent (this MonoBehaviour sender, EventID eventID)
		{
			EventDispatcher.Instance.PostEvent(eventID, sender, null);
		}
	}
	#endregion
}