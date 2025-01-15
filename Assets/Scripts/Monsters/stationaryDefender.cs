using UnityEngine;

public class stationaryDefender : Monster //한자리를 지키고 있는 몬스터(=일반타워)
{

    protected override void Scaring()
    {
        if (_isSingleTargetAttack)
        {
            base.Scaring();
            return;
        }

        foreach (Human human in TargetHumanList)
        {
            if (human == null) continue;

            if (_lastScareTime >= data.cooldown)
            {
                MonsterProjectile projectile = PoolManager.Instance.SpawnFromPool<MonsterProjectile>(_projectileData.projectileType.ToString(), transform.position, Quaternion.identity);
                projectile.SetProjectileInfo(human.transform.position, data, _projectileData);
                StartCoroutine(ShowBooText());
                _lastScareTime = 0f;
                SetState(MonsterState.Idle);
            }
        }
    }
}
