import {
  ApiPhoneVerificationSendCodePostRequest,
  ApiPhoneVerificationVerifyCodePostRequest,
} from "../codegen";
import { Result } from "../types/common.types";
import {
  getErrorsFromApiResult,
  isStatusCodeSuccess,
} from "../utils/common.utils";
import { logError } from "../utils/logging.utils";
import { PhoneVerificationApiWithConfig } from "./api.service";

export class PhoneVerificationService {
  protected static phoneVerificationApi = new PhoneVerificationApiWithConfig();

  public static async verifyPhoneNumber({
    request,
  }: {
    request: ApiPhoneVerificationSendCodePostRequest;
  }): Promise<Result<boolean>> {
    try {
      const response =
        await this.phoneVerificationApi.apiPhoneVerificationSendCodePostRaw(
          request
        );

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      return {
        success: true,
        data: true,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error verifying phone number",
        "VerifyPhoneNumberError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error verifying phone number"],
      };
    }
  }

  public static async verifyCodeNumber({
    request,
  }: {
    request: ApiPhoneVerificationVerifyCodePostRequest;
  }): Promise<Result<boolean>> {
    try {
      const response =
        await this.phoneVerificationApi.apiPhoneVerificationVerifyCodePostRaw(
          request
        );

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }

      return {
        success: true,
        data: true,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error verifying phone number",
        "VerifyPhoneNumberError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error verifying phone number"],
      };
    }
  }
}
