using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    public static CharacterControllerScript Current;

    public float limitX;
    public float xSpeed;

    public float runningSpeed;
    float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<CylinderClass> cylinders;

    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private bool _shouldSpawn;

    private float _bridgeTimer;

    public Animator animator;

    float _scoreTimer;
    bool _finished;

    float _lastTouchPosX;

    public AudioSource cylinderAudioSource , triggerAudioSource , itemAudioSource;
    public AudioClip cylinderGather, cylinderDrop , buyItemClip , equipItemAudioClip , unEquipItemAudioClip;
    public AudioClip coinClip;
    private float _dropAudioTimer;

    public List<GameObject> wearSpots;

    void Update()
    {
        if( LevelController.Current == null || LevelController.Current.isGameActive == false)
        {
            return;
        }

        float newX = 0;
        float touchDeltaX = 0;
        if (Input.touchCount > 0)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchPosX = Input.GetTouch(0).position.x;
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchDeltaX = 45 * (Input.GetTouch(0).position.x - _lastTouchPosX)/Screen.width;
                _lastTouchPosX = Input.GetTouch(0).position.x;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            touchDeltaX = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + (xSpeed * touchDeltaX * Time.deltaTime);
        newX = Mathf.Clamp(newX, -limitX, limitX);
        Vector3 newPos = new Vector3(newX, transform.position.y, transform.position.z + Time.deltaTime + _currentRunningSpeed);
        transform.position = newPos;


        if (_shouldSpawn)
        {
            _bridgeTimer -= Time.deltaTime;
            if(_bridgeTimer < 0)
            {
                DropCyclinder();
                _bridgeTimer = 0.02f;
                IncrementCylinderVolume(-0.02f);
               
                GameObject newBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                newBridgePiece.transform.forward = direction;
                
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + characterDistance * direction;
                newPiecePosition.x = transform.position.x;
                newBridgePiece.transform.position = newPiecePosition;

                if(_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if(_scoreTimer < 0)
                    {
                        _scoreTimer = 0.01f;
                        LevelController.Current.changeScore(1);
                    }
                }
            }
        }


    
    }

    public void ChangeSpeed(float speedValue)
    {
        _currentRunningSpeed = speedValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(cylinderGather , 0.1f);
            Destroy(other.gameObject);
            IncrementCylinderVolume(0.13f);
        }
        else if(other.tag == "SpawnBridge")
        {
            StartSpawnBridge(other.gameObject.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if(other.tag == "StopSpawnBridge")
        {
            if (_finished)
            {
                LevelController.Current.FinishLevel();
            }
            StopSpawnBridge();
        }else if(other.tag == "Finish")
        {
            _finished = true;
            StartSpawnBridge(other.gameObject.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if(other.tag == "Coin")
        {
            triggerAudioSource.PlayOneShot(coinClip, 0.1f);
            other.tag = "Untagged";
            LevelController.Current.changeScore(10);
            Destroy(other.gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.isGameActive)
        {
            if (other.tag == "Trap")
            {
                DropCyclinder();
                IncrementCylinderVolume(-Time.fixedDeltaTime);
            }
        }
    }

    public void IncrementCylinderVolume(float addingVolume)
    {
        if(cylinders.Count == 0)
        {
            if(addingVolume > 0)
            {
                CreateCylinder(addingVolume);
            }
            else
            {
                if (_finished)
                {
                    LevelController.Current.FinishLevel();
                }
                else
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].InclemetCylinderVolume(addingVolume);
        }
    }

    public void Die()
    {
        LevelController.Current.Gameover();
        gameObject.layer = 6;
        Camera.main.transform.parent = null;
        animator.SetBool("dead", true);
    }

    public void CreateCylinder(float volume)
    {
        CylinderClass createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<CylinderClass>();
        cylinders.Add(createdCylinder);
        createdCylinder.InclemetCylinderVolume(volume);
    }
    public void DestroyCylinder(CylinderClass cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }

    public void StartSpawnBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _shouldSpawn = true;
    }
    
    public void StopSpawnBridge()
    {
        _shouldSpawn = false;
    }

    public void DropCyclinder()
    {
        _dropAudioTimer -= Time.deltaTime;
        if(_dropAudioTimer < 0)
        {
            _dropAudioTimer = 0.05f;
            cylinderAudioSource.PlayOneShot(cylinderDrop, 0.15f);
        }
    }
}
