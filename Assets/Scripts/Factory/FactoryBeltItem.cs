using Assets.Scripts.Factory.Base;
using UnityEngine;

namespace Assets.Scripts.Factory {
    [RequireComponent(typeof(SpriteRenderer))]
    public class FactoryBeltItem : MonoBehaviour {
        public FactoryBeltItem NextPosition = null;
        public FactoryBeltItem PreviousPosition = null;
        public ItemObject Item;
        public Vector3 PathForAnimation = new Vector3(0, 0.25f);
        public float MaxOffset = 0.4f;
        public float Offset;
        public float Speed = 0.5f;

        private SpriteRenderer _spriteRenderer;
        private Sprite _baseSprite;

        private Vector3 _basePosition;
        private bool _freeze;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _baseSprite = _spriteRenderer.sprite;
            if (Item != null) _spriteRenderer.sprite = Item.Sprite;
            _basePosition = transform.localPosition;
        }

        private void Start()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void Update()
        {
            if (_freeze || Item == null || (NextPosition == null && Offset >= MaxOffset)) return;
            Offset += Time.deltaTime * Speed;
            if (Offset >= MaxOffset) {
                Offset = MaxOffset;
                if (NextPosition != null) {
                    if (NextPosition.Item == null) {
                        MoveItem();
                    } else {
                        _freeze = true;
                    }
                }
            }
            transform.localPosition = _basePosition - PathForAnimation + PathForAnimation * Offset / MaxOffset;
        }

        public void SetItem(ItemObject item)
        {
            Item = item;
            Offset = 0;
            _spriteRenderer.sprite = Item.Sprite;
        }

        public void RemoveItem()
        {
            Item = null;
            Offset = 0;
            _spriteRenderer.sprite = _baseSprite;
            UpdatePreviousItem();
        }

        public void GiveItem(ItemObject item)
        {
            if (Item == null) SetItem(item);
        }

        public void MoveItem()
        {
            if (NextPosition == null) return;
            Offset = MaxOffset;
            if (NextPosition.Item == null) {
                NextPosition.SetItem(Item);
                RemoveItem();
            }
        }

        private void UpdatePreviousItem()
        {
            if (PreviousPosition != null) PreviousPosition._freeze = false;
        }

        public void OnBreakInput()
        {
            if (PreviousPosition != null) {
                PreviousPosition.NextPosition = null;
                PreviousPosition._freeze = false;
            }
        }

        public void OnBreakOutput()
        {
            if (NextPosition != null) NextPosition.PreviousPosition = null;
        }
    }
}