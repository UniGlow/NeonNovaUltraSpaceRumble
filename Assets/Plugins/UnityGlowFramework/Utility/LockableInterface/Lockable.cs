using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of the ILockable Interface.
/// </summary>
[System.Serializable]
public class Lockable : ILockable
{

    List<Component> locks = new List<Component>();
    bool isLocked = false;
    public bool Islocked { get { return isLocked; } }

    /// <summary>
    /// Adds a lock to the current class.
    /// </summary>
    /// <param name="locking">Set or release lock.</param>
    /// <param name="raiser">Component executing the lock.</param>
	public void Lock(bool locking, Component raiser)
    {
        if (locking == true)
        {
            if (!locks.Contains(raiser))
            {
                locks.Add(raiser);
                isLocked = true;
            }
        }
        else if (locking == false)
        {
            locks.Remove(raiser);
            if (locks.Count == 0) isLocked = false;
        }
    }

    /// <summary>
    /// Releases all locks from the current class. Only call this if you are sure you want to release it in any possible state.
    /// </summary>
    public void ReleaseAll()
    {
        locks.Clear();
        isLocked = false;
    }
}
