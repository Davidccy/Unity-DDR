using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class UINodeHandler : MonoBehaviour {
    [System.Serializable]
    public class NodeRootSet {
        public GameObject GoRoot;
        public UINodeRoot NodeLeft;
        public UINodeRoot NodeUp;
        public UINodeRoot NodeDown;
        public UINodeRoot NodeRight;
        public UINodeRoot NodeSpace;
    }

    [System.Serializable]
    public class HitEffectRootSet {
        public ParentConstraint PCLeft;
        public ParentConstraint PCUp;
        public ParentConstraint PCDown;
        public ParentConstraint PCRight;
        public ParentConstraint PCSpace;
    }

    #region Serialized Fields
    [SerializeField] private NodeRootSet _nodeRootSetRaising = null;
    [SerializeField] private NodeRootSet _nodeRootSetFalling = null;

    [SerializeField] private HitEffectRootSet _hitEffectRootSet = null;

    [SerializeField] private UINode _uiNodeRes = null;
    #endregion

    #region Internal Fields
    private int _nodeMovingType;
    private float _nodeSpeed;
    private NodeRootSet _nodeRootSet = null;
    private Dictionary<NodePosition, List<UINode>> _nodeMap = new Dictionary<NodePosition, List<UINode>>();
    private float _nodeHandlingThreshold = 0.15f;
    private float _finalNodeTiming = 0;
    private bool _isNodeFinished = false;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        GameEventManager.Instance.Register(GameEventTypes.TRACK_LOADED, OnTrackLoaded);
        GameEventManager.Instance.Register(GameEventTypes.NODE_PRESSED, OnNodePressed);
    }

    private void Update() {
        CheckIsNodeFinished();
    }
    
    private void OnDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.TRACK_LOADED, OnTrackLoaded);
            GameEventManager.Instance.Unregister(GameEventTypes.NODE_PRESSED, OnNodePressed);
        }
    }
    #endregion

    #region Event Handlings
    private void OnTrackLoaded(BaseGameEventArgs args) {
        InitSettings();
        InitUI();

        RemoveNotes();
        GenerateNodes();
    }

    private void OnNodePressed(BaseGameEventArgs args) {
        NodePressedGameEventArgs npArgs = args as NodePressedGameEventArgs;
        NodePosition np = npArgs.NP;
        NodeHandling(np);
    }
    #endregion

    #region Internal Methods
    private void NodeHandling(NodePosition np) {
        if (!_nodeMap.ContainsKey(np)) {
            return;
        }

        // Find nearest node
        int nodeIndex = -1;
        float minDiffTiming = 100;
        float curTiming = TrackManager.Instance.TrackProgress;
        List<UINode> uiNodeList = _nodeMap[np];
        for (int i = 0; i < uiNodeList.Count; i++) {
            if (uiNodeList[i].IsDone) {
                continue;
            } 

            float diffTiming = uiNodeList[i].NDInfo.Timing - curTiming;
            if (Mathf.Abs(diffTiming) > _nodeHandlingThreshold) {
                continue;
            }

            if (diffTiming < minDiffTiming) {
                minDiffTiming = diffTiming;
                nodeIndex = i;
            }
        }

        if (nodeIndex == -1) {
            return;
        }

        // Set done
        UINode node = _nodeMap[np][nodeIndex];
        node.IsDone = true;

        // Show node result on node root
        NodeResult result = GetNodeResult(minDiffTiming);
        NodeResultGameEventArgs args = new NodeResultGameEventArgs();
        args.NR = result;
        args.NP = np;
        args.Dispatch();

        // Hide(or destroy) node
        node.gameObject.SetActive(false);
        _nodeMap[np].RemoveAt(nodeIndex);

        // Sound effect
        TrackManager.Instance.PlaySE(result == NodeResult.Perfect);
    }

    private NodeResult GetNodeResult(float diffTiming) {
        NodeResult result = NodeResult.Miss;

        if (Mathf.Abs(diffTiming) <= 0.03f) {
            result = NodeResult.Perfect;
        }
        else if (Mathf.Abs(diffTiming) <= 0.10f) {
            result = NodeResult.Great;
        }
        else if (Mathf.Abs(diffTiming) <= _nodeHandlingThreshold) {
            result = NodeResult.Good;
        }

        return result;
    }

    private void InitSettings() {
        _nodeMovingType = Utility.GetNodeMovingType();
        _nodeSpeed = Utility.GetTrackSpeedValue();
    }

    private void InitUI() {
        // UI node roots set
        _nodeRootSet = _nodeMovingType == (int) NodeMovingType.Raising ? _nodeRootSetRaising : _nodeRootSetFalling;
        _nodeRootSetRaising.GoRoot.SetActive(_nodeMovingType == (int) NodeMovingType.Raising);
        _nodeRootSetFalling.GoRoot.SetActive(_nodeMovingType == (int) NodeMovingType.Falling);

        _nodeRootSet.NodeLeft.Activate();
        _nodeRootSet.NodeUp.Activate();
        _nodeRootSet.NodeDown.Activate();
        _nodeRootSet.NodeRight.Activate();
        _nodeRootSet.NodeSpace.Activate();

        // Hit effect
        _hitEffectRootSet.PCLeft.AddSource(new ConstraintSource() { sourceTransform = _nodeRootSet.NodeLeft.transform, weight = 1 });
        _hitEffectRootSet.PCUp.AddSource(new ConstraintSource() { sourceTransform = _nodeRootSet.NodeUp.transform, weight = 1 });
        _hitEffectRootSet.PCDown.AddSource(new ConstraintSource() { sourceTransform = _nodeRootSet.NodeDown.transform, weight = 1 });
        _hitEffectRootSet.PCRight.AddSource(new ConstraintSource() { sourceTransform = _nodeRootSet.NodeRight.transform, weight = 1 });
        _hitEffectRootSet.PCSpace.AddSource(new ConstraintSource() { sourceTransform = _nodeRootSet.NodeSpace.transform, weight = 1 });
    }

    private void RemoveNotes() {
        if (_nodeMap != null) {
            foreach (List<UINode> nodeList in _nodeMap.Values) {
                for (int i = 0; i < nodeList.Count; i++) {
                    Destroy(nodeList[i].gameObject);
                }
            }

            _nodeMap.Clear();
        }
        else {
            _nodeMap = new Dictionary<NodePosition, List<UINode>>();
        }
    }

    private void GenerateNodes() {
        // Track data
        TrackData tData = TrackManager.Instance.TrackData;

        // BPM
        int bpm = tData.BPM;
        float spm = 60.0f / (float) bpm * tData.BumpPerMeasure; // Second per measure

        for (int i = 0; i < tData.Nodes.Length; i++) {
            NodeData nData = tData.Nodes[i];
            int measure = nData.Measure;
            for (int j = 0; j < nData.NodeInfoList.Length; j++) {
                NodeInfo info = nData.NodeInfoList[j];

                NodeDisplayInfo nodedisplayInfo = new NodeDisplayInfo();
                nodedisplayInfo.Position = info.NodePosition;
                nodedisplayInfo.MovingType = (NodeMovingType) _nodeMovingType;
                nodedisplayInfo.Timing = spm * ((measure - 1) + info.Timing) + tData.FirstMeasure;
                nodedisplayInfo.Speed = _nodeSpeed;

                UINodeRoot root = GetUINodeRoot(info.NodePosition);
                if (root == null) {
                    continue;
                }

                UINode uiNode = Instantiate(_uiNodeRes, root.transform);
                uiNode.SetInfo(root, nodedisplayInfo, _nodeHandlingThreshold, OnNodeMissed, OnNodeOutOfBound);

                if (!_nodeMap.ContainsKey(info.NodePosition)) {
                    _nodeMap.Add(info.NodePosition, new List<UINode>());
                }

                _nodeMap[info.NodePosition].Add(uiNode);

                if (_finalNodeTiming < nodedisplayInfo.Timing) {
                    _finalNodeTiming = nodedisplayInfo.Timing;
                }
            }
        }

        NodeGeneratedGameEventArgs args = new NodeGeneratedGameEventArgs();
        args.Dispatch();
    }

    private UINodeRoot GetUINodeRoot(NodePosition np) {
        UINodeRoot root = null;

        if (np == NodePosition.Left) {
            root = _nodeRootSet.NodeLeft;
        }
        else if (np == NodePosition.Up) {
            root = _nodeRootSet.NodeUp;
        }
        else if (np == NodePosition.Down) {
            root = _nodeRootSet.NodeDown;
        }
        else if (np == NodePosition.Right) {
            root = _nodeRootSet.NodeRight;
        }
        else if (np == NodePosition.Space) {
            root = _nodeRootSet.NodeSpace;
        }

        return root;
    }

    private void OnNodeMissed(UINode node) {
        // Set done
        node.IsDone = true;

        // Show node result on node root
        NodeResultGameEventArgs args = new NodeResultGameEventArgs();
        args.NR = NodeResult.Miss;
        args.NP = node.NDInfo.Position;
        args.Dispatch();
    }

    private void OnNodeOutOfBound(UINode node) {
        // Hide(or destroy) node
        node.gameObject.SetActive(false);
        _nodeMap[node.NDInfo.Position].Remove(node);
    }

    private void CheckIsNodeFinished() {
        if (!TrackManager.Instance.IsPlaying) {
            return;
        }

        if (_isNodeFinished) {
            return;
        }

        if (TrackManager.Instance.TrackProgress < _finalNodeTiming) {
            return;
        }

        _isNodeFinished = true;

        FinalNodeFinishedGameEventArgs args = new FinalNodeFinishedGameEventArgs();
        args.Dispatch();
    }
    #endregion
}
