using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GateLogic : MonoBehaviour
{
    [SerializeField] private TriggerForward firstTrigger;
    [SerializeField] private TriggerForward secondTrigger;

    private Zone _firstZone;
    private Zone _secondZone;
    [SerializeField] private ComposedGate firstGate;// IGate would be nicer but Interfaces are not serializable
    [SerializeField] private ComposedGate secondGate;

    private void Awake() {
        _firstZone = new Zone(firstTrigger);
        _secondZone = new Zone(secondTrigger);
    }
    private void OnEnable() {
        _firstZone.Enable();
        _secondZone.Enable();
    }

    private void OnDisable() {
        _firstZone.Disable();
        _secondZone.Disable();
    }

    private void Update() {
        if (_secondZone.MemberState == Zone.State.OnlyAuthorized) {
            firstGate.SetOpening(false);
            if (firstGate.IsClosed())
                secondGate.SetOpening(true);
            return;
        }
        bool authorizedWantsToEnter = _firstZone.MemberState == Zone.State.Mixed || _firstZone.MemberState == Zone.State.OnlyAuthorized;
        bool unauthorizedInside = _secondZone.MemberState == Zone.State.Mixed ||_firstZone.MemberState == Zone.State.OnlyUnauthorized;
        if (authorizedWantsToEnter || unauthorizedInside) {
            if (secondGate.IsClosed()) {
                secondGate.SetOpening(false);
                firstGate.SetOpening(true);
                return;
            }
        }
        secondGate.SetOpening(false);
        firstGate.SetOpening(false);
    }

    private class Zone {
        private HashSet<Collider> authorized;
        private HashSet<Collider> unauthorized;
        private TriggerForward trigger;

        public enum State {
            NoOne, OnlyAuthorized, OnlyUnauthorized, Mixed
        }
        private State _state;
        public State MemberState => _state;

        public void Add(Collider collider) {
            bool allowed = true;// Todo determine if allowed
            if (allowed)
                authorized.Add(collider);
            else
                unauthorized.Add(collider);
            RecalculateState();
        }

        public void Remove(Collider collider) {
            if (authorized.Contains(collider))
                authorized.Remove(collider);
            else
                unauthorized.Remove(collider);
            RecalculateState();
        }

        private void Subscribe(TriggerForward trigger) {
            trigger.OnEnter += Add;
            trigger.OnExit += Remove;
        }

        private void Unsubscribe(TriggerForward trigger) {
            trigger.OnEnter -= Add;
            trigger.OnExit -= Remove;
        }

        public void Enable() {
            Subscribe(trigger);
        }

        public void Disable() {
            Unsubscribe(trigger);
        }

        public Zone(TriggerForward trigger) {
            this.trigger = trigger;
            _state = State.NoOne;
        }

        private void RecalculateState() {
            if (authorized.Count == 0) {
                if (unauthorized.Count == 0)
                    _state = State.NoOne;
                else
                    _state = State.OnlyUnauthorized;
            }
            else {
                if (unauthorized.Count == 0)
                    _state = State.OnlyUnauthorized;
                else
                    _state = State.Mixed;
            }
        }
    }
}
