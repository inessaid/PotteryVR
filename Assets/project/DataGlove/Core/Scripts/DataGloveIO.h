/*! \file
    \brief The Data Glove API. The DataGloveIO class handles all of the possible inputs and outputs of the
    BeBop Sensors Data Glove.

    To receive data, call the methods GetSensors() and
    GetIMU() for finger bend amounts and rotation orientation.
*/


#ifndef DATAGLOVEIO_H
#define DATAGLOVEIO_H

//------------------------------------------------------------------------------
// Utility Methods
//------------------------------------------------------------------------------

/// Sets the glove to be left or right handed
/**
 */
void SetHandType(bool isRightHand);

/// Properly disconnects the glove
/**
 */
void Close();

/// Returns the battery status percentage
/**
*/
float GetBatteryStatus();

/// Returns the firmware version
/**
*/
string GetFirmware();

/// Returns the bootloader version
/**
*/
string GetBootloader();

/// Returns the hardware rev
/**
*/
string GetHardwareRev();

/// Returns the hardware version
/**
*/
string GetHardwareVersion();

/// Returns the BLE module version
/**
*/
string GetBLEFirmwareVersion();

/// Returns the BLE bootloader version
/**
*/
string GetBLEBootloaderVersion();

/// Returns the BLE soft device version
/**
*/
string GetBLESoftDeviceVersion();

/// Returns the connection status
/**
*/
bool IsConnected();

//------------------------------------------------------------------------------
// Sensor Methods
//------------------------------------------------------------------------------

/// Returns the 5 FingerVals structs as an array of FingerVals
/**
 * Each FingerVals object contains
 * sensor1 - Calibrated sensor 1 value - The knuckle sensor
 * sensor2 - Calibrated sensor 2 value - The 2nd joint sensor
 */
FingerVals[] GetSensors();

/// Sets sensor values from 7 bit to 12 bit (7 bit is default)
/**
 * true - make 12 bit
 * false - make 7 bit
 */
void SetSensor12Bit(bool make12Bit);

//------------------------------------------------------------------------------
// Calibration Methods
//------------------------------------------------------------------------------

/// Resets the sensor calibration
/**
 */
void ResetCalibration();

/// Calibrates the sensor values when hand is held out flat
/**
 */
void CalibrateFlat();

/// Calibrates the sensor values when hand is held out flat with thumb in
/**
 */
void CalibrateThumbIn();

/// Calibrates the sensor values when the thumb is out and four fingers are in.
/**
 */
void CalibrateFingersIn();

/// Saves the calibration to a slot (0-5)
/**
 */
void SaveCalibration(int slot);

/// Recalls the calibrated slot (0-5)
/**
 */
void RecallCalibration(int slot);

//------------------------------------------------------------------------------
// IMU Methods
//------------------------------------------------------------------------------

/// Returns the calibrated Quaternion from the glove
/**
 * The imu is the inertial measurement unit on the Dataglove, which gives
 * quaternion values (w, x, y, z).
 */
Quaternion GetIMU();

/// Sets the new home position of the IMU
/**
 */
void SetIMUHome();

//------------------------------------------------------------------------------
// Haptic Methods
//------------------------------------------------------------------------------

/// Trigger a single one shot haptic
/**
 * Given an actuator ID (0-5) set a note and an amplitude
 */
void SendOneShotHaptic(int actuatorID, int note, float amplitude);

/// Trigger a looping haptic
/**
 * Given an actuator ID (0-5) set a note and an amplitude
 */
void SendLoopHaptic(int actuatorID, int note, float amplitude);

/// Given an actuator ID (0-5) set an grain location (0.0f-1.0f)
/**
 */
void SetGrainLocation(int actuatorID, float location);

/// Given an actuator ID (0-5) set an amplitude (0-127)
/**
 */
void SetHapticAmp(int actuatorID, int amplitude);

/// Given an actuator ID (0-5) set a pitch bend (0-127)
/**
 */
void SetPitchBend(int actuatorID, int pitch);

/// Given an actuator ID (0-5) set a grain size (0-127)
/**
 */
void SetGrainSize(int actuatorID, int size);

/// Given an actuator ID (0-5) set a grain fade (0-127)
/**
 */
void SetGrainFade(int actuatorID, int fade);

/// Silences all actuators
/**
 * This function is called automatically by OnApplicationQuit().
 */
void SilenceHaptics();

/// Turns on haptics. (haptics are enabled by default)
/**
 * true - turns haptics on
 * false - turns haptics off
 */
void ToggleHapticsOn(bool turnOn);

/// Changes a given acuator's sound file.
/**
*/
void SelectHapticWave(int actuatorID, int waveform);

#endif
