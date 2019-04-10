using System;
using System.Collections.Generic;
using Urho;
using Urho.Physics;
using Urho.Urho2D;

namespace SmartRoadSense.Shared {

    public class VehicleCreator {
        RigidBody2D _mainBody;
        ConstraintWheel2D _wheel1;
        ConstraintWheel2D _wheel2;
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

            // Outer body for collision - null mass
            CollisionChain2D bodyBounds = box.CreateComponent<CollisionChain2D>();
            bodyBounds.VertexCount = 9;
            bodyBounds.SetVertex(0, new Vector2(-1.5f * _screenInfo.XScreenRatio, -0.7f * _screenInfo.YScreenRatio)); // BOTTOM LEFT
            bodyBounds.SetVertex(1, new Vector2(-1.5f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));
            bodyBounds.SetVertex(2, new Vector2(-0.3f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));
            bodyBounds.SetVertex(3, new Vector2(-0.3f * _screenInfo.XScreenRatio, -0.5f * _screenInfo.YScreenRatio));
            bodyBounds.SetVertex(4, new Vector2(0.0f * _screenInfo.XScreenRatio, -0.5f * _screenInfo.YScreenRatio));
            bodyBounds.SetVertex(5, new Vector2(0.0f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));
            bodyBounds.SetVertex(6, new Vector2(0.5f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));
            bodyBounds.SetVertex(7, new Vector2(0.5f * _screenInfo.XScreenRatio, -0.7f * _screenInfo.YScreenRatio));  // BOTTOM RIGHT
            bodyBounds.SetVertex(8, new Vector2(-1.5f * _screenInfo.XScreenRatio, -0.7f * _screenInfo.YScreenRatio)); // BOTTOM LEFT   
            bodyBounds.Friction = 1.0f;
            bodyBounds.Restitution = 0.0f;

            CollisionChain2D backBounds = box.CreateComponent<CollisionChain2D>();
            backBounds.VertexCount = 4;
            backBounds.SetVertex(0, new Vector2(-1.5f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));
            backBounds.SetVertex(1, new Vector2(-1.5f * _screenInfo.XScreenRatio, -0.4f * _screenInfo.YScreenRatio));
            backBounds.SetVertex(2, new Vector2(-1.45f * _screenInfo.XScreenRatio, -0.4f * _screenInfo.YScreenRatio));
            backBounds.SetVertex(3, new Vector2(-1.45f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));
            backBounds.SetVertex(4, new Vector2(-1.5f * _screenInfo.XScreenRatio, -0.6f * _screenInfo.YScreenRatio));

            backBounds.Friction = 1.0f;
            backBounds.Restitution = 0.0f;

            // Main body for constraints and mass
            // Create box shape
            CollisionBox2D shape = box.CreateComponent<CollisionBox2D>(); 
            // Set size
            shape.Size = new Vector2(0.32f * _screenInfo.XScreenRatio, 0.32f * _screenInfo.YScreenRatio); 
            shape.Density = 8.0f;      // Set shape density (kilograms per meter squared)
            shape.Friction = 0.8f;      // Set friction
            shape.Restitution = 0.0f;   // Set restitution (no bounce)

            // Update center of collision body - moves center of mass
            shape.SetCenter(-0.3f * _screenInfo.XScreenRatio, -1.0f * _screenInfo.YScreenRatio);   

            // Create a ball (will be cloned later)
            Node ball1WheelNode = scene.CreateChild("Wheel");
            StaticSprite2D ballSprite = ball1WheelNode.CreateComponent<StaticSprite2D>();

            if(_vehicleModel.WheelsPosition.Count == 1) {
                l = _vehicleModel.WheelsPosition[0].Left;
                t = _vehicleModel.WheelsPosition[0].Top;
                r = _vehicleModel.WheelsPosition[0].Right;
                b = _vehicleModel.WheelsPosition[0].Bottom;
            }
            else
            {
                // TODO if vehicle has different wheel textures
            }

            sprite = new Sprite2D {
                Texture = cache.GetTexture2D("Textures/Garage/vehicles_wheels.png"),
                Rectangle = new IntRect(l, t, r, b)
            };

            // Create a vehicle from a compound of 2 ConstraintWheel2Ds
            var car = box;
            car.Scale = new Vector3(1.5f * _screenInfo.XScreenRatio, 1.5f * _screenInfo.YScreenRatio, 0.0f);

            // DEFINES POSITION OF SPRITE AND MAIN BODY MASS
            car.Position = new Vector3(0.0f * _screenInfo.XScreenRatio, _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);

            // DEFINE WHEELS
            ballSprite.Sprite = sprite;

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
            Node ball2WheelNode = ball1WheelNode.Clone(CreateMode.Replicated);
            StaticSprite2D ball2Sprite = ball2WheelNode.CreateComponent<StaticSprite2D>();
            ball2Sprite.Sprite = sprite;

            // TODO: change for more than 2 wheels
            for(var i = 0; i < _vehicleModel.WheelsBodyPosition.Count; i++) {
                if(i == 0) {
                    float x1 = _vehicleModel.WheelsBodyPosition[i].X % _vehicleImgPos;
                    float y1 = _vehicleModel.WheelsBodyPosition[i].Y % _vehicleImgPos;
                    x1 = x1 >= _vehicleImgPos / 2 ? (x1 - _vehicleImgPos / 2) * Application.PixelSize : -(_vehicleImgPos / 2 - x1) * Application.PixelSize;
                    y1 = (_vehicleImgPos / 2 - y1) * Application.PixelSize;

                    // WHEEL POSITION - NEEDS TO BE SET RELATIVE TO MAIN BODY
                    ball1WheelNode.Position = new Vector3(x1, y1 + _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);
                    ball1WheelNode.Scale = new Vector3(1.7f * _screenInfo.XScreenRatio, 1.7f * _screenInfo.YScreenRatio, 0.0f);
                }
                if(i == 1) {
                    float x2 = _vehicleModel.WheelsBodyPosition[i].X % _vehicleImgPos;
                    float y2 = _vehicleModel.WheelsBodyPosition[i].Y % _vehicleImgPos;
                    x2 = x2 >= _vehicleImgPos / 2 ? (x2 - _vehicleImgPos / 2) * Application.PixelSize : -(_vehicleImgPos / 2 - x2) * Application.PixelSize;
                    y2 = (_vehicleImgPos / 2 - y2) * Application.PixelSize;

                    // WHEEL POSITION - NEEDS TO BE SET RELATIVE TO MAIN BODY
                    ball2WheelNode.Position = new Vector3(x2, y2 + _vehicleCenterYOffset * _screenInfo.YScreenRatio, 1.0f);
                    ball2WheelNode.Scale = new Vector3(1.7f * _screenInfo.XScreenRatio, 1.7f * _screenInfo.YScreenRatio, 0.0f);
                }
            }

            // SET BACK WHEEL CONSTRAINT COMPONENTS
            _wheel1 = car.CreateComponent<ConstraintWheel2D>();
            _wheel1.OtherBody = ball1WheelNode.GetComponent<RigidBody2D>();
            _wheel1.Anchor = ball1WheelNode.Position2D;
            _wheel1.Axis = new Vector2(0.0f, 1.0f);
            _wheel1.MaxMotorTorque = 150.0f;
            _wheel1.FrequencyHz = 15.0f;
            _wheel1.DampingRatio = 10.0f;
            _wheel1.MotorSpeed = 0.0f;

            // SET FRONT WHEEL CONSTRAINT COMPONENTS
            _wheel2 = car.CreateComponent<ConstraintWheel2D>();
            _wheel2.OtherBody = ball2WheelNode.GetComponent<RigidBody2D>();
            _wheel2.Anchor = ball2WheelNode.Position2D;
            _wheel2.Axis = new Vector2(0.0f, 1.0f);
            _wheel2.MaxMotorTorque = 150.0f;
            _wheel2.FrequencyHz = 15.0f;
            _wheel2.DampingRatio = 10.0f;
            _wheel2.MotorSpeed = 0.0f;
        }

        public Vehicle InitCarInScene(VehicleLoad vehicleLoad) {
            return new Vehicle(_mainBody, _wheel1, _wheel2, _vehicleModel, vehicleLoad);
        }
    }

    public class Vehicle
    {
        public RigidBody2D MainBody { get; private set; }
        public ConstraintWheel2D Wheel1 { get; private set; }
        public ConstraintWheel2D Wheel2 { get; private set; }
        public VehicleModel SelectedVehicle { get; private set; }
        public VehicleLoad SelectedVehicleLoad { get; private set; }
        public Node BalanceObject { get; set; }

        public Vehicle(RigidBody2D mainBody, ConstraintWheel2D wheel1, ConstraintWheel2D wheel2, VehicleModel vehicleType, VehicleLoad vehicleLoad)
        {
            MainBody = mainBody;
            Wheel1 = wheel1;
            Wheel2 = wheel2;
            SelectedVehicle = vehicleType;
            SelectedVehicleLoad = vehicleLoad;
        }
    }
}
