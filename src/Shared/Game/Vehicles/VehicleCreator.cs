using System;
using System.Collections.Generic;
using Urho;
using Urho.Physics;
using Urho.Urho2D;
using System.Linq;

namespace SmartRoadSense.Shared {

    public class VehicleCreator {
        RigidBody2D _mainBody;
        List<ConstraintWheel2D> _wheelsConstraints = new List<ConstraintWheel2D>();
        VehicleModel _vehicleModel;
        ScreenInfoRatio _screenInfo;
        readonly float _vehicleImgPos = 250;
        readonly float _vehicleCenterYOffset = 0.6f;

        public VehicleCreator(Scene scene, Urho.Resources.ResourceCache cache, ScreenInfoRatio screenInfo) {
            _screenInfo = screenInfo;

            // Load vehicle info
            _vehicleModel = VehicleManager.Instance.SelectedVehicleModel;

            var l = _vehicleModel.BodyPosition.Left;
            var t = _vehicleModel.BodyPosition.Top;
            var r = _vehicleModel.BodyPosition.Right;
            var b = _vehicleModel.BodyPosition.Bottom;

            // Get vehicle texture from coordinates
            var sprite = new Sprite2D {
                Texture = cache.GetTexture2D("Textures/Garage/vehicles_body.png"),
                Rectangle = new IntRect(l, t, r, b)
            };

            // Create vehicle main body reference
            Node box = scene.CreateChild("Box");

            StaticSprite2D boxSprite = box.CreateComponent<StaticSprite2D>();
            boxSprite.Sprite = sprite;

            _mainBody = box.CreateComponent<RigidBody2D>();
            _mainBody.BodyType = BodyType2D.Dynamic;
            _mainBody.LinearDamping = 0.2f;
            _mainBody.AngularDamping = 0.2f;
            _mainBody.Bullet = true;
            _mainBody.Mass = 1.0f;
            //_mainBody.SetMassCenter(new Vector2(-2.1f * _screenInfo.XScreenRatio, -0.1f * _screenInfo.YScreenRatio));

            /****************************************/
            /* Outer body for collision - null mass */
            /****************************************/
            float startVX = -1.2f;
            float startVY = -0.55f;
            CollisionChain2D bodyBounds = box.CreateComponent<CollisionChain2D>();
            bodyBounds.VertexCount = 9;
            bodyBounds.SetVertex(0, new Vector2(startVX, startVY)); // BOTTOM LEFT
            bodyBounds.SetVertex(1, new Vector2(startVX, startVY + 0.1f));
            bodyBounds.SetVertex(2, new Vector2(startVX + 1.0f, startVY + 0.1f));
            bodyBounds.SetVertex(3, new Vector2(startVX + 1.0f, startVY + 0.2f));
            bodyBounds.SetVertex(4, new Vector2(startVX + 1.3f, startVY + 0.2f));
            bodyBounds.SetVertex(5, new Vector2(startVX + 1.3f, startVY + 0.1f));
            bodyBounds.SetVertex(6, new Vector2(startVX + 1.8f, startVY + 0.1f));
            bodyBounds.SetVertex(7, new Vector2(startVX + 1.8f, startVY));  // BOTTOM RIGHT
            bodyBounds.SetVertex(8, new Vector2(startVX, startVY)); // BOTTOM LEFT   
            bodyBounds.Friction = 1.0f;
            bodyBounds.Restitution = 0.0f;

            CollisionChain2D backBounds = box.CreateComponent<CollisionChain2D>();
            backBounds.VertexCount = 4;
            backBounds.SetVertex(0, new Vector2(startVX, startVY + 0.1f));
            backBounds.SetVertex(1, new Vector2(startVX, startVY + 0.3f));
            backBounds.SetVertex(2, new Vector2(startVX + 0.05f, startVY + 0.3f));
            backBounds.SetVertex(3, new Vector2(startVX + 0.05f, startVY + 0.1f));
            backBounds.SetVertex(4, new Vector2(startVX, startVY + 0.1f));

            backBounds.Friction = 1.0f;
            backBounds.Restitution = 0.0f;

            // Main body for constraints and mass
            // Create box shape
            CollisionBox2D shape = box.CreateComponent<CollisionBox2D>(); 
            // Set size
            shape.Size = new Vector2(0.32f, 0.32f); 
            shape.Density = 8.0f;      // Set shape density (kilograms per meter squared)
            shape.Friction = 0.8f;      // Set friction
            shape.Restitution = 0.0f;   // Set restitution (no bounce)

            // Update center of collision body - moves center of mass
            shape.SetCenter(-0.2f, -0.65f);   

            // Create a vehicle from a compound of 2 ConstraintWheel2Ds
            var car = box;
            //car.Scale = new Vector3(1.5f * _screenInfo.XScreenRatio, 1.5f * _screenInfo.YScreenRatio, 0.0f);

            // DEFINES POSITION OF SPRITE AND MAIN BODY MASS
            car.Position = new Vector3(0.0f * _screenInfo.XScreenRatio, _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);

            /*****************/
            /* DEFINE WHEELS */
            /*****************/


            var wheelSprites = new List<Sprite2D>();
            foreach(var w in _vehicleModel.WheelsPosition) {
                wheelSprites.Add(new Sprite2D {
                    Texture = cache.GetTexture2D("Textures/Garage/vehicles_wheels.png"),
                    Rectangle = new IntRect(w.Left, w.Top, w.Right, w.Bottom)
                });
            }
            // Create a ball (will be cloned later)
            Node ball1WheelNode = scene.CreateChild("Wheel");
            StaticSprite2D ballSprite = ball1WheelNode.CreateComponent<StaticSprite2D>();
            ballSprite.Sprite = wheelSprites[0];

            RigidBody2D ballBody = ball1WheelNode.CreateComponent<RigidBody2D>();
            ballBody.BodyType = BodyType2D.Dynamic;
            ballBody.LinearDamping = 0.1f;
            ballBody.AngularDamping = 0.1f;
            ballBody.Bullet = true;
            ballBody.Mass = 2.0f;

            // WHEEL COLLISION
            CollisionCircle2D ballShape = ball1WheelNode.CreateComponent<CollisionCircle2D>(); // Create circle shape
            ballShape.Radius = 0.14f; // Set radius
            ballShape.Density = 2.0f; // Set shape density (kilograms per meter squared)
            ballShape.Friction = (float)_vehicleModel.Wheel / 10.0f; // Set friction: 1.0 = max friction
            ballShape.Restitution = (float)_vehicleModel.Suspensions / 10.0f; // Set restitution: make it bounce: 0.0 = no bounce

            // CLONE AND POSITION WHEELS

            for(var i = 0; i < _vehicleModel.WheelsBodyPosition.Count; i++) {
                if(i == 0) 
                {
                    float x1 = _vehicleModel.WheelsBodyPosition[i].X % _vehicleImgPos;
                    float y1 = _vehicleModel.WheelsBodyPosition[i].Y % _vehicleImgPos;
                    x1 = x1 >= _vehicleImgPos / 2 ? (x1 - _vehicleImgPos / 2) * Application.PixelSize : -(_vehicleImgPos / 2 - x1) * Application.PixelSize;
                    y1 = (_vehicleImgPos / 2 - y1) * Application.PixelSize;

                    // WHEEL POSITION - NEEDS TO BE SET RELATIVE TO MAIN BODY
                    ball1WheelNode.Position = new Vector3(x1, y1 + _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);

                    // SET BACK WHEEL CONSTRAINT COMPONENTS
                    var constraintWheel = car.CreateComponent <ConstraintWheel2D>();
                    constraintWheel.OtherBody = ball1WheelNode.GetComponent<RigidBody2D>();
                    constraintWheel.Anchor = ball1WheelNode.Position2D;
                    constraintWheel.Axis = new Vector2(0.0f, 1.0f);
                    constraintWheel.MaxMotorTorque = 150.0f;
                    constraintWheel.FrequencyHz = 15.0f;
                    constraintWheel.DampingRatio = 10.0f;
                    constraintWheel.MotorSpeed = 0.0f;
                    _wheelsConstraints.Add(constraintWheel);
                } 
                else 
                {
                    Node newWheelNode = ball1WheelNode.Clone(CreateMode.Replicated);
                    StaticSprite2D newBallSprite = newWheelNode.CreateComponent<StaticSprite2D>();
                    newBallSprite.Sprite = wheelSprites.Count > i ? wheelSprites[i] : wheelSprites.Last();

                    float x2 = _vehicleModel.WheelsBodyPosition[i].X % _vehicleImgPos;
                    float y2 = _vehicleModel.WheelsBodyPosition[i].Y % _vehicleImgPos;
                    x2 = x2 >= _vehicleImgPos / 2 ? (x2 - _vehicleImgPos / 2) * Application.PixelSize : -(_vehicleImgPos / 2 - x2) * Application.PixelSize;
                    y2 = (_vehicleImgPos / 2 - y2) * Application.PixelSize;

                    // WHEEL POSITION - NEEDS TO BE SET RELATIVE TO MAIN BODY
                    newWheelNode.Position = new Vector3(x2, y2 + _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);

                    // SET FRONT WHEEL CONSTRAINT COMPONENTS
                    var constraintWheel = car.CreateComponent<ConstraintWheel2D>();
                    constraintWheel.OtherBody = newWheelNode.GetComponent<RigidBody2D>();
                    constraintWheel.Anchor = newWheelNode.Position2D;
                    constraintWheel.Axis = new Vector2(0.0f, 1.0f);
                    constraintWheel.MaxMotorTorque = 150.0f;
                    constraintWheel.FrequencyHz = 15.0f;
                    constraintWheel.DampingRatio = 10.0f;
                    constraintWheel.MotorSpeed = 0.0f;
                    _wheelsConstraints.Add(constraintWheel);
                }
            }


            // TODO: change for more than 2 wheels
            /*
            for(var i = 0; i < _vehicleModel.WheelsBodyPosition.Count; i++) {
                if(i == 0) {
                    float x1 = _vehicleModel.WheelsBodyPosition[i].X % _vehicleImgPos;
                    float y1 = _vehicleModel.WheelsBodyPosition[i].Y % _vehicleImgPos;
                    x1 = x1 >= _vehicleImgPos / 2 ? (x1 - _vehicleImgPos / 2) * Application.PixelSize : -(_vehicleImgPos / 2 - x1) * Application.PixelSize;
                    y1 = (_vehicleImgPos / 2 - y1) * Application.PixelSize;

                    // WHEEL POSITION - NEEDS TO BE SET RELATIVE TO MAIN BODY
                    ball1WheelNode.Position = new Vector3(x1, y1 + _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);
                    //ball1WheelNode.Scale = new Vector3(1.7f * _screenInfo.XScreenRatio, 1.7f * _screenInfo.YScreenRatio, 0.0f);
                }
                if(i == 1) {
                    float x2 = _vehicleModel.WheelsBodyPosition[i].X % _vehicleImgPos;
                    float y2 = _vehicleModel.WheelsBodyPosition[i].Y % _vehicleImgPos;
                    x2 = x2 >= _vehicleImgPos / 2 ? (x2 - _vehicleImgPos / 2) * Application.PixelSize : -(_vehicleImgPos / 2 - x2) * Application.PixelSize;
                    y2 = (_vehicleImgPos / 2 - y2) * Application.PixelSize;

                    // WHEEL POSITION - NEEDS TO BE SET RELATIVE TO MAIN BODY
                    ball2WheelNode.Position = new Vector3(x2, y2 + _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);
                    //ball2WheelNode.Scale = new Vector3(1.7f * _screenInfo.XScreenRatio, 1.7f * _screenInfo.YScreenRatio, 0.0f);
                }
            }
            */
        }

        public Vehicle InitCarInScene(VehicleLoad vehicleLoad) {
            return new Vehicle(_mainBody, _wheelsConstraints, _vehicleModel, vehicleLoad);
        }
    }

    public class Vehicle
    {
        public RigidBody2D MainBody { get; private set; }
        public List<ConstraintWheel2D> WheelConstraints { get; private set; }
        public VehicleModel SelectedVehicle { get; private set; }
        public VehicleLoad SelectedVehicleLoad { get; private set; }
        public Node BalanceObject { get; set; }

        public Vehicle(RigidBody2D mainBody, List<ConstraintWheel2D> wheelConstraints, VehicleModel vehicleType, VehicleLoad vehicleLoad)
        {
            MainBody = mainBody;
            WheelConstraints = wheelConstraints;
            SelectedVehicle = vehicleType;
            SelectedVehicleLoad = vehicleLoad;
        }
    }
}
