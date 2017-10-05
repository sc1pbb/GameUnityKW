using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticlesPoolManager : MonoBehaviour
{
    public static ParticlesPoolManager singleton_ { get; private set; }

    public List<ProjectileController> projectilePrefabs_ = new List<ProjectileController> ();
    public List<ProjectileController> projectilesSpawned_ = new List<ProjectileController> ();

    void Awake ()
    {
        if (singleton_ == null)
            singleton_ = this;
    }

    public ProjectileController GetFirstProjectileAvaible (ProjectileType projectileType)
    {
        //First try to see if there is any already spawned enemy that has the correct enemytype
        foreach (ProjectileController projectile in projectilesSpawned_)
        {
            if (projectile.projectileType_ == projectileType && projectile.isActive_ == false)
                return projectile;
        }

        //If we didn't find any possible enemy spawn one
        ProjectileController projectileToSpawn = projectilePrefabs_[0];
        //Find the type;
        foreach (ProjectileController projectileByTpe in projectilePrefabs_)
        {
            if (projectileByTpe.projectileType_ == projectileType)
                projectileToSpawn = projectileByTpe;
        }

        //Now spawn that object and add it to our spawned enemies.
        projectileToSpawn = (ProjectileController)Instantiate (projectileToSpawn, Vector3.zero, Quaternion.identity);
        projectilesSpawned_.Add (projectileToSpawn);

        return projectileToSpawn;
    }
}
