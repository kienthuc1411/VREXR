﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GCP_CREDENTIAL
{
    /*public string type;
    public string project_id;
    public string private_key_id;
    public string private_key;
    public string client_email;
    public string client_id;
    public string auth_uri;
    public string token_uri;
    public string auth_provider_x509_cert_url;
    public string client_x509_cert_url;*/

    public string type ="service_account";
    public string project_id ="vr-e-learning";
    public string private_key_id = "343cef9f40135a11cd820af709a45f59fef2e0fd";
    public string private_key ="-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQD5MS5jGsGHyI5D\n70c/E8jMbX3RdUyQHDjq/0js32xZggHtIZuaIPHJ5Ew0hgCVY73d+zsLeUDtohTu\nTpSMiZ3FfxjTEeoPzJ/ievrXPEX8RNp2CQusSgD+pfXxb89uYw/dj83Jj5CNf7OZ\nmIDZQnBXCuymVlg5X+AkMup3sg/KxXkwMeZXslLumQ/n2XB/fcIJTK786H9QkJJz\nlfybPqwqdqVA8L5ALsID3HAFxND0GKIFT4SJbIw9dFXY8+RnbZ1GC96DSyuihJOo\n4KYSyydAckxgVrr4fJb6UwTc9gDReC5+riW9p8QuEulZB45fwuBdTGikigq7BKzD\nO1JVBsFjAgMBAAECggEAY9u0CY3iyP1zGUEtTVcpy9YCqem2DTfqDS3Q9O9RlYrW\nJ8ckej7CddOxQAkE+diW8Poovp/QB4pAlgDSYYESQt78PTT8FWm8Fn+OvqrOJtoW\nq19TmK6tOF6cMKzMNKGo7XIflqqb1EA84gutcqK/rvnXPI70WsWTZh4rPF0UG/58\n8/YHAK1YVTRJQpQIDwXZAYLR3VsUzMGe/HXl77LwDAlkO9bK8gCyPkwGrZhvxJQL\nzNDsGPokBfGdN8b2dsNq8eqQAxVxLwH7XEsRnEz/H0CASavPKtGMkV/s4LigU/Ea\ndKC84rgi4fkNklLonEPbR2Ij9eE2xDu1UqKv85od1QKBgQD9zaGO6esgISbk6SFc\ngW9C4Ldh+uBx3tAJXegtzFt0xOlivQ6AHQczY1Sy09+NVJ5coB5bO34hBB3XPQCP\n+zr5W5X3sbBjJZWi/XKXhBXcRTkQRwlhCCTpCw5oxFodYlA4yruJ+coJAXbQwT34\n7Wi1CbRAUIpKJC/NGxrhaao8PwKBgQD7WVU6CZKl9jq2ofdq5dlaDN4BatlzseDc\ni0WcQqNpANB7g0i1S7GpJBfEWnqqCZVlNVfU2xnDHRerN0Trvo2AdwcaytNVAc2d\nJzjVzDIkh2oTTsg83q1vr7K8vClNopm3hlON8XdBzdkE3VyzwW5bO/2yRebwISH5\nDRa9wguB3QKBgQDyPE7ULiKBeeK7XF7BAbCFbiDY3S9Wv0hjiENPYtpvKnluCe9L\nC2mR/F4ch+e35ml3EkOm47NQI7OveMOOqEPzNxx9WhR1rKuS8r9qKWEL6O27wXEM\nMU+5NRo9mBpCLVFPwv8Xg1b3HXSfbbCOY1kqYOau86/pb3xIEHSpa8ZU1QKBgAsZ\nYMgVUCKAAwu5j0FrMPPnCY6qdzvCqRlUFRjXYHvsi0hI/dbzpr6/V0VWcYA8uBom\nBuDhY/vXtwnagPKgEYOQvgGS304rrDa6WqomQDiYujhsDC+T7bi223+2F2TUP1F5\nXwsvlIKVYnXyiHtvmT3yIjvTFmWYMPXcfBqZDQUJAoGBAL2Z1aHRMK0NJG31NrOZ\nJtU8acw7KiHatY3WRWTTK8OArHVpEYqWiwZsI9PILE6g025I8UoVmWhpYgoRFkto\nNcSGwKbxo27HLhjj/kd9OgOnXUONtE0iFrY8k/pu6pxBlihcrQ+Iot/Uwbn2nkQp\neKN1UAGLnQZHLmECR4kKvkV+\n-----END PRIVATE KEY-----\n";
    public string client_email ="speech-to-text-realtime@vr-e-learning.iam.gserviceaccount.com";
    public string client_id = "107528737464733700998";
    public string auth_uri ="https://accounts.google.com/o/oauth2/auth";
    public string token_uri ="https://oauth2.googleapis.com/token";
    public string auth_provider_x509_cert_url ="https://www.googleapis.com/oauth2/v1/certs";
    public string client_x509_cert_url ="https://www.googleapis.com/robot/v1/metadata/x509/speech-to-text-realtime%40vr-e-learning.iam.gserviceaccount.com";
    
    public GCP_CREDENTIAL()
    {
    /*    type = "service_account";
        project_id = "vr-e-learning";
        private_key_id = "343cef9f40135a11cd820af709a45f59fef2e0fd";
        private_key = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQD5MS5jGsGHyI5D\n70c/E8jMbX3RdUyQHDjq/0js32xZggHtIZuaIPHJ5Ew0hgCVY73d+zsLeUDtohTu\nTpSMiZ3FfxjTEeoPzJ/ievrXPEX8RNp2CQusSgD+pfXxb89uYw/dj83Jj5CNf7OZ\nmIDZQnBXCuymVlg5X+AkMup3sg/KxXkwMeZXslLumQ/n2XB/fcIJTK786H9QkJJz\nlfybPqwqdqVA8L5ALsID3HAFxND0GKIFT4SJbIw9dFXY8+RnbZ1GC96DSyuihJOo\n4KYSyydAckxgVrr4fJb6UwTc9gDReC5+riW9p8QuEulZB45fwuBdTGikigq7BKzD\nO1JVBsFjAgMBAAECggEAY9u0CY3iyP1zGUEtTVcpy9YCqem2DTfqDS3Q9O9RlYrW\nJ8ckej7CddOxQAkE+diW8Poovp/QB4pAlgDSYYESQt78PTT8FWm8Fn+OvqrOJtoW\nq19TmK6tOF6cMKzMNKGo7XIflqqb1EA84gutcqK/rvnXPI70WsWTZh4rPF0UG/58\n8/YHAK1YVTRJQpQIDwXZAYLR3VsUzMGe/HXl77LwDAlkO9bK8gCyPkwGrZhvxJQL\nzNDsGPokBfGdN8b2dsNq8eqQAxVxLwH7XEsRnEz/H0CASavPKtGMkV/s4LigU/Ea\ndKC84rgi4fkNklLonEPbR2Ij9eE2xDu1UqKv85od1QKBgQD9zaGO6esgISbk6SFc\ngW9C4Ldh+uBx3tAJXegtzFt0xOlivQ6AHQczY1Sy09+NVJ5coB5bO34hBB3XPQCP\n+zr5W5X3sbBjJZWi/XKXhBXcRTkQRwlhCCTpCw5oxFodYlA4yruJ+coJAXbQwT34\n7Wi1CbRAUIpKJC/NGxrhaao8PwKBgQD7WVU6CZKl9jq2ofdq5dlaDN4BatlzseDc\ni0WcQqNpANB7g0i1S7GpJBfEWnqqCZVlNVfU2xnDHRerN0Trvo2AdwcaytNVAc2d\nJzjVzDIkh2oTTsg83q1vr7K8vClNopm3hlON8XdBzdkE3VyzwW5bO/2yRebwISH5\nDRa9wguB3QKBgQDyPE7ULiKBeeK7XF7BAbCFbiDY3S9Wv0hjiENPYtpvKnluCe9L\nC2mR/F4ch+e35ml3EkOm47NQI7OveMOOqEPzNxx9WhR1rKuS8r9qKWEL6O27wXEM\nMU+5NRo9mBpCLVFPwv8Xg1b3HXSfbbCOY1kqYOau86/pb3xIEHSpa8ZU1QKBgAsZ\nYMgVUCKAAwu5j0FrMPPnCY6qdzvCqRlUFRjXYHvsi0hI/dbzpr6/V0VWcYA8uBom\nBuDhY/vXtwnagPKgEYOQvgGS304rrDa6WqomQDiYujhsDC+T7bi223+2F2TUP1F5\nXwsvlIKVYnXyiHtvmT3yIjvTFmWYMPXcfBqZDQUJAoGBAL2Z1aHRMK0NJG31NrOZ\nJtU8acw7KiHatY3WRWTTK8OArHVpEYqWiwZsI9PILE6g025I8UoVmWhpYgoRFkto\nNcSGwKbxo27HLhjj/kd9OgOnXUONtE0iFrY8k/pu6pxBlihcrQ+Iot/Uwbn2nkQp\neKN1UAGLnQZHLmECR4kKvkV+\n-----END PRIVATE KEY-----\n";
        client_email = "speech-to-text-realtime@vr-e-learning.iam.gserviceaccount.com";
        client_id = "107528737464733700998";
        auth_uri = "https://accounts.google.com/o/oauth2/auth";
        token_uri = "https://oauth2.googleapis.com/token";
        auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
        client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/speech-to-text-realtime%40vr-e-learning.iam.gserviceaccount.com";
    */
    }
}
