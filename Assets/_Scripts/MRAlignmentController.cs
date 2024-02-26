using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MRAlignmentController : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject RoomPrefab;

    List<(GameObject, OVRLocatable)> _locatableObjects = new List<(GameObject, OVRLocatable)>();

    // Start is called before the first frame update
    void Start()
    {
        SceneManagerHelper.RequestScenePermission();
        LoadSceneAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    async void LoadSceneAsync()
    {
        // fetch all rooms, with a SceneCapture fallback
        var rooms = new List<OVRAnchor>();
        await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms);
        if (rooms.Count == 0)
        {
            var sceneCaptured = await SceneManagerHelper.RequestSceneCapture();
            if (!sceneCaptured)
                return;

            await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms);
        }

        // fetch room elements, create objects for them
        var tasks = rooms.Select(async room =>
        {
            var roomObject = new GameObject($"Room-{room.Uuid}");
            if (!room.TryGetComponent(out OVRAnchorContainer container))
                return;
            if (!room.TryGetComponent(out OVRRoomLayout roomLayout))
                return;

            var children = new List<OVRAnchor>();
            await container.FetchChildrenAsync(children);
            await CreateSceneAnchors(roomObject, roomLayout, children);
        }).ToList();
        await Task.WhenAll(tasks);
    }

    async Task CreateSceneAnchors(GameObject roomGameObject,
    OVRRoomLayout roomLayout, List<OVRAnchor> anchors)
    {
        roomLayout.TryGetRoomLayout(out var ceilingUuid,
            out var floorUuid, out var wallUuids);

        // iterate over all anchors as async tasks
        var tasks = anchors.Select(async anchor =>
        {
            // can we locate it in the world?
            if (!anchor.TryGetComponent(out OVRLocatable locatable))
                return;
            await locatable.SetEnabledAsync(true);

            // check room layout information and assign prefab
            // it would also be possible to use the semantic label
            
            if (anchor.Uuid == floorUuid)
            {
                var prefab = FloorPrefab;
                // get semantic classification for object name
                var label = "other";
                if (anchor.TryGetComponent(out OVRSemanticLabels labels))
                    label = labels.Labels;

                // create container object
                var gameObject = new GameObject(label);
                gameObject.transform.SetParent(roomGameObject.transform);
                var helper = new SceneManagerHelper(gameObject);
                helper.SetLocation(locatable);

                // instantiate prefab & set 2D dimensions
                var model = Instantiate(prefab, gameObject.transform);
                if (anchor.TryGetComponent(out OVRBounded2D bounds2D) &&
                    bounds2D.IsEnabled)
                {
                    model.transform.localScale = new Vector3(
                        bounds2D.BoundingBox.size.x,
                        bounds2D.BoundingBox.size.y,
                        0.01f);
                }

                var room = Instantiate(RoomPrefab, gameObject.transform);
            }

            /*

            // we will set volume dimensions for the non-room elements
            if (prefab == FallbackPrefab)
            {
                if (anchor.TryGetComponent(out OVRBounded3D bounds3D) &&
                    bounds3D.IsEnabled)
                {
                    model.transform.localPosition = new Vector3(0, 0,
                        -bounds3D.BoundingBox.size.z / 2);
                    model.transform.localScale = bounds3D.BoundingBox.size;
                }
            }
            */

            // save game object and locatable for updating later
            _locatableObjects.Add((gameObject, locatable));
        }).ToList();

        await Task.WhenAll(tasks);
    }



}
