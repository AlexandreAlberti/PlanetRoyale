using System;
using UnityEngine;

namespace Application.Telemetry
{
    public class FirebaseCrashlyticsTester : MonoBehaviour
    {
        private int _updatesBeforeException;

        // Use this for initialization
        void Start()
        {
            _updatesBeforeException = 0;
        }

        // Update is called once per frame
        void Update()
        {
            // Call the exception-throwing method here so that it's run
            // every frame update
            throwExceptionEvery60Updates();
        }

        // A method that tests your Crashlytics implementation by throwing an
        // exception every 60 frame updates. You should see reports in the
        // Firebase console a few minutes after running your app with this method.
        void throwExceptionEvery60Updates()
        {
            if (_updatesBeforeException > 0)
            {
                _updatesBeforeException--;
            }
            else
            {
                // Set the counter to 60 updates
                _updatesBeforeException = 60;

                // Throw an exception to test your Crashlytics implementation
                throw new Exception("test exception please ignore");
            }
        }
    }
}