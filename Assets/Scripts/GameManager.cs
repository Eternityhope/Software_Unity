using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject itemShop;
    public GameObject startZone;
    public GameObject weaponShop;

    public Player player;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI playTimeTxt;
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerAmmoTxt;
    public TextMeshProUGUI playerCoinTxt;

    void Awake()
    {
        stage = 1;
        enemyList = new List<int>();
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }
    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle =false;
        stage++;
    }

    IEnumerator InBattle()
    {
        for (int index = 0; index < stage; index++)
        {
            int ran = Random.Range(0, 3);
            enemyList.Add(ran);

            switch (ran)
            {
                case 0:
                    enemyCntA++;
                    break;
                case 1:
                    enemyCntB++;
                    break;
                case 2:
                    enemyCntC++;
                    break;
            }
        }
        while (enemyList.Count > 0)
        {
            int ranZone = Random.Range(0, 4);
            GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            enemyList.RemoveAt(0);
            yield return new WaitForSeconds(5);
        }
        while (enemyCntA + enemyCntB + enemyCntC > 0)
        {
            yield return new WaitForSeconds(4f);
        }
        StageEnd();
    }

    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }



    void LateUpdate()
    {
        stageTxt.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}",hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);

        if (player.equipWeapon != null)
        {
            // 일단 무기가 있으면, 그 무기의 종류에 따라 다르게 표시
            if (player.equipWeapon.type == Weapon.Type.Melee)
            {
                // 근접 무기일 때는 보통 탄창 개념이 없으므로 다른 표시 (예: 무한대 또는 빈칸)
                playerAmmoTxt.text = "∞";
            }
            else // Weapon.Type.Range 등 다른 타입일 경우
            {
                // 원거리 무기일 때만 현재 탄창과 전체 탄약을 표시
                playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;
            }
        }
        else // 무기를 아예 장착하지 않았을 경우
        {
            // 빈 칸으로 깔끔하게 처리
            playerAmmoTxt.text = "";
        }


    }
}
