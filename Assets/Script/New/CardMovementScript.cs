using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera _mainCamera;
    private Vector3 _offset;
    private Transform _defaultParent,_defaultTempCardParent;
    GameObject TempCardGO;
    private bool _isDraggable;
    private GameManagerScript _gameManager;
    public GameManagerScript GameManager
    {
        get { return _gameManager; }
    }

    public Transform DefaultParent
    {
        get
        {
            return _defaultParent;
        }
        set
        {
            _defaultParent = value;
        }
    }

    public Transform DefaultTempCardParent
    {
        get
        {
            return _defaultTempCardParent;
        }
        set
        {
            _defaultTempCardParent = value;
        }
    }

    private void Awake()
    {
        _mainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
        _gameManager = FindObjectOfType<GameManagerScript>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //calculate offset
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);

        //store current parent
        _defaultParent = _defaultTempCardParent = transform.parent;

        _isDraggable = GameManager.IsPlayerTurn && (
                                                     (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.PLAYER_HAND &&
                                                     GameManager.PlayerMana >= GetComponent<CardInfoScript>()._selfCard.manacost) ||
                                                     (
                                                      DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.PLAYER_BOARD &&
                                                      GetComponent<CardInfoScript>()._selfCard.canAttack
                                                     )

                                                    );

        if (!_isDraggable)
            return;

        
        if(GetComponent<CardInfoScript>()._selfCard.canAttack)
            GameManager.HighliteTargets(true);

        TempCardGO.transform.SetParent(_defaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        //dissatach from previous parent
        transform.SetParent(_defaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;

        Vector3 newPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        
        transform.position = newPos + _offset;

        if (TempCardGO.transform.parent != _defaultTempCardParent)
            TempCardGO.transform.SetParent(_defaultTempCardParent);

        if(_defaultParent.GetComponent<DropPlaceScript>().Type != FieldType.PLAYER_BOARD)
            CheckPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;

        GameManager.HighliteTargets(false);

        transform.SetParent(_defaultParent);

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(3055, 0);
    }

    private void CheckPosition()
    {
        int newIndex = _defaultTempCardParent.childCount;

        for(int i = 0; i < _defaultTempCardParent.childCount; i ++)
        {
            if(transform.position.x < _defaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                {
                    newIndex--;
                }

                break;
            }
        }

        TempCardGO.transform.SetSiblingIndex(newIndex);
    }

    public void MovetoField(Transform field)
    {
        transform.DOMove(field.position, .5f);
    }

    public void MovetoTarget(Transform target)
    {
        StartCoroutine(MoveToTargetCor(target));
    }

    IEnumerator MoveToTargetCor(Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();

        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        transform.SetParent(GameObject.Find("Canvas").transform);

        transform.DOMove(target.position, .25f);

        yield return new WaitForSeconds(.25f);

        transform.DOMove(pos, .25f);

        yield return new WaitForSeconds(.25f);

        transform.SetParent(parent);
        transform.SetSiblingIndex(index);

        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
