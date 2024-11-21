using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler,
    IEndDragHandler, IPointerDownHandler
{
    Grid gridObject;
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f);
    private Vector3 parmak = new Vector3(0, 650f, 0);

    [HideInInspector] public ShapeData CurrentShapeData;

    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentShapes = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;

    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;
    public int num;


    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        _startPosition = _transform.localPosition;
        _shapeActive = true;
        gridObject = FindObjectOfType<Grid>();
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }


    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShapes)
        {
            if (square.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    public void DeactiveShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShapes)
            {
                square?.GetComponent<ShapeSquare>().DeactiveShape();
            }
        }

        _shapeActive = false;
    }

    private void SetShapeInactive()
    {
        if (IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShapes)
            {
                square.gameObject.SetActive(false);
            }
        }
    }


    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShapes)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }

        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData) //*********************************************************
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }


    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquarers(shapeData);

        while (_currentShapes.Count <= TotalSquareNumber)
            _currentShapes.Add(Instantiate(squareShapeImage, transform) as GameObject);

        foreach (var square in _currentShapes)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x * 0.8f,
            squareRect.rect.height * squareRect.localScale.y *
            0.8f); //þeklin durduðu zemin üzerindeki aralýklarý ayarlayan kýsým

        int currentIndexInList = 0;

        //set positions to form final shape - sekli generate eden kisim

        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShapes[currentIndexInList].SetActive(true);
                    _currentShapes[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                            GetYPositionForShapeSquare(shapeData, row, moveDistance));

                    currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < middleSquareIndex) // eksiltiyor
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }

                else if (row > middleSquareIndex) //arttirma
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 2;
                var multiplier = shapeData.rows / 2;

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if (row == middleSquareIndex2)
                    {
                        shiftOnY = (moveDistance.y / 2) * -1;
                    }

                    if (row == middleSquareIndex1)
                    {
                        shiftOnY = (moveDistance.y / 2);
                    }

                    if (row < middleSquareIndex1 && row < middleSquareIndex2)
                    {
                        shiftOnY = moveDistance.y * 1;
                        shiftOnY = multiplier;
                    }

                    else if (row > middleSquareIndex1 && row > middleSquareIndex2)
                    {
                        shiftOnY = moveDistance.y * -1;
                        shiftOnY *= multiplier;
                    }
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if (shapeData.columns > 1) //Dikey pozisyon hesaplama
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIndex) //move it on the negative
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex) //move it on plus
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns / 2;

                if (column == middleSquareIndex1 ||
                    column == middleSquareIndex2) // iki durumu || or ile tek if icinde kontrol et
                {
                    if (column == middleSquareIndex2)
                    {
                        shiftOnX = (moveDistance.x) / 2;
                    }

                    if (column == middleSquareIndex1)
                    {
                        shiftOnX = (moveDistance.x / 2) * -1;
                    }
                }

                if (column > middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
        }

        return shiftOnX;
    }

    private int GetNumberOfSquarers(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                    number++;
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.instance.Play("pop");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale =
            shapeSelectedScale * 1.15f; //þeklin ele alýndýktan sonraki boyutunu deðiþtiren kýsým.
        //AudioManager.instance.Play("pop");
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);


        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);

        _transform.localPosition = pos + offset;
        _transform.localPosition += parmak;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale =
            _shapeStartScale; //ilk þekilden sonra gelen þeklin boyutunu deðiþtir
        if (gameObject.name == "Shape")
        {
            num = 0;
        }
        else if (gameObject.name == "Shape (1)")
        {
            num = 1;
        }
        else if (gameObject.name == "Shape (2)")
        {
            num = 2;
        }

        //AudioManager.instance.Play("pop");
        gridObject.callis(num);
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.instance.Play("pop");
    }

    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }
}