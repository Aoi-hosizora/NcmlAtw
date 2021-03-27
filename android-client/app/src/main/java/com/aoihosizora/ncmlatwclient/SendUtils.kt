package com.aoihosizora.ncmlatwclient

import android.text.TextUtils
import android.util.Patterns
import androidx.annotation.WorkerThread
import java.io.PrintWriter
import java.lang.Exception
import java.net.Socket

object SendUtils {
    var ip: String? = null
    var port: Int? = null

    fun checkIPString(s: String): Boolean {
        return Patterns.IP_ADDRESS.matcher(s).matches()
    }

    fun checkPortString(s: String): Boolean {
        val p: Int
        try {
            p = Integer.parseInt(s)
        } catch (ex: Exception) {
            return false
        }
        if (p < 1 || p > 65535) {
            return false
        }
        return true
    }

    @WorkerThread
    fun ping(): Boolean {
        if (ip == null || port == null) {
            return true
        }

        try {
            val socket = Socket(ip!!, port!!)
            socket.close()
        } catch (ex: Exception) {
            return false
        }
        return true
    }

    enum class SendResult {
        SUCCESS,
        FAILED,
        WRONG_PARAM,
    }

    @WorkerThread
    fun send(text: String): SendResult {
        if (ip == null || port == null || TextUtils.isEmpty(text)) {
            return SendResult.WRONG_PARAM
        }

        try {
            val socket = Socket(ip!!, port!!)
            val writer = PrintWriter(socket.getOutputStream(), true)
            writer.println(text)
            socket.close()
        } catch (ex: Exception) {
            return SendResult.FAILED
        }

        return SendResult.SUCCESS
    }
}
