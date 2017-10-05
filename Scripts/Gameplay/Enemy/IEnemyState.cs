using UnityEngine;
using System.Collections;

public interface IEnemyState
{
    void StartState ();

    void UpdateState ();

    void ToDisableState ();

    void ToEnablingState ();

    void ToChaseState ();

    void ToAttackState ();
}
