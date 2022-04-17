using System;
using System.Collections.Generic;
using UnityEngine;

public class UINodeHandler : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private UINodeRoot _uiNodeLeft = null;
    [SerializeField] private UINodeRoot _uiNodeUp = null;
    [SerializeField] private UINodeRoot _uiNodeDown = null;
    [SerializeField] private UINodeRoot _uiNodeRight = null;
    [SerializeField] private UINodeRoot _uiNodeSpace = null;

    [SerializeField] private UINode _uiNodeRes = null;
    #endregion

    #region Exposed Fields
    public Action onNodeGenerated = null;
    public Action onFinalNodeFinished = null;
    #endregion

    #region Internal Fields
    private float _speed = 3.0f;
    private Dictionary<NodePosition, List<UINode>> _nodeMap = new Dictionary<NodePosition, List<UINode>>();
    private float _nodeHandlingThreshold = 0.15f;
    private float _finalNodeTiming = 0;
    private bool _isNodeFinished = false;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        TrackManager.Instance.onTrackLoaded += OnTrackLoaded;
    }

    private void OnEnable() {
        EventManager.Instance.Register(EventTypes.NODE_PRESSED, OnNodePressed);
    }

    private void Update() {
        CheckIsNodeFinished();
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
        if (onFinalNodeFinished != null) {
            onFinalNodeFinished();
        }
    }

    private void OnDisable() {
        if (EventManager.Instance != null) {
            EventManager.Instance.Unregister(EventTypes.NODE_PRESSED, OnNodePressed);
        }        
    }

    private void OnDestroy() {
        if (TrackManager.Instance != null) {
            TrackManager.Instance.onTrackLoaded -= OnTrackLoaded;
        }        
    }
    #endregion

    #region Event Handlings
    private void OnNodePressed(BaseEventArgs args) {
        NodePressedEventArgs npArgs = args as NodePressedEventArgs;
        NodePosition np = npArgs.NP;
        NodeHandling(np);
    }
    #endregion

    #region Callback Handlings
    private void OnTrackLoaded() {
        RemoveNotes();
        GenerateNodes();
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

            float diffTiming = uiNodeList[i].NInfo.Timing - curTiming;
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

        // Show tap result on node root
        TapResult result = GetTapResult(minDiffTiming);
        TapResultEventArgs args = new TapResultEventArgs();
        args.TR = result;
        args.NP = np;
        args.Dispatch();

        // Hide(or destroy) node
        node.gameObject.SetActive(false);
        _nodeMap[np].RemoveAt(nodeIndex);

        // Sound effect
        TrackManager.Instance.PlaySE(result == TapResult.Perfect);
    }

    private TapResult GetTapResult(float diffTiming) {
        TapResult result = TapResult.Miss;

        if (Mathf.Abs(diffTiming) <= 0.03f) {
            result = TapResult.Perfect;
        }
        else if (Mathf.Abs(diffTiming) <= 0.10f) {
            result = TapResult.Great;
        }
        else if (Mathf.Abs(diffTiming) <= _nodeHandlingThreshold) {
            result = TapResult.Good;
        }

        return result;
    }

    private void RemoveNotes() {
        foreach (List<UINode> nodeList in _nodeMap.Values) {
            for (int i = 0; i < nodeList.Count; i++) {
                Destroy(nodeList[i].gameObject);
            }
        }

        _nodeMap.Clear();
    }

    private void GenerateNodes() {
        TrackData td = TrackManager.Instance.TrackData;

        int bpm = td.BPM;
        float timePerMeasure = 60.0f / (float) bpm * 4;

        for (int i = 0; i < td.Nodes.Length; i++) {
            NodeData nData = td.Nodes[i];
            int measure = nData.Measure;
            for (int j = 0; j < nData.NodeInfoList.Length; j++) {
                NodeInfo info = nData.NodeInfoList[j];

                NodeInfoTest nodeInfo = new NodeInfoTest();
                nodeInfo.Position = info.NodePosition;
                nodeInfo.Timing = timePerMeasure * ((measure - 1) + info.Timing) + td.FirstMeasure;
                nodeInfo.Speed = _speed;

                UINodeRoot root = GetUINodeRoot(info.NodePosition);
                if (root == null) {
                    continue;
                }

                UINode uiNode = Instantiate(_uiNodeRes, root.transform);
                uiNode.SetInfo(root, nodeInfo, _nodeHandlingThreshold, OnNodeMissed, OnNodeOutOfBound);

                if (!_nodeMap.ContainsKey(info.NodePosition)) {
                    _nodeMap.Add(info.NodePosition, new List<UINode>());
                }

                _nodeMap[info.NodePosition].Add(uiNode);

                if (_finalNodeTiming < nodeInfo.Timing) {
                    _finalNodeTiming = nodeInfo.Timing;
                }
            }
        }

        if (onNodeGenerated != null) {
            onNodeGenerated();
        }
    }

    private UINodeRoot GetUINodeRoot(NodePosition np) {
        UINodeRoot root = null;

        if (np == NodePosition.Left) {
            root = _uiNodeLeft;
        }
        else if (np == NodePosition.Up) {
            root = _uiNodeUp;
        }
        else if (np == NodePosition.Down) {
            root = _uiNodeDown;
        }
        else if (np == NodePosition.Right) {
            root = _uiNodeRight;
        }
        else if (np == NodePosition.Space) {
            root = _uiNodeSpace;
        }

        return root;
    }

    private void OnNodeMissed(UINode node) {
        // Set done
        node.IsDone = true;

        // Show tap result on node root
        TapResultEventArgs args = new TapResultEventArgs();
        args.TR = TapResult.Miss;
        args.NP = node.NInfo.Position;
        args.Dispatch();
    }

    private void OnNodeOutOfBound(UINode node) {
        // Hide(or destroy) node
        node.gameObject.SetActive(false);
        _nodeMap[node.NInfo.Position].Remove(node);
    }
    #endregion
}
