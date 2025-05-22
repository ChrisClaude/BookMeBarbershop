import { Result } from "../types/common.types";

export const transformRTKResult = <T>(result: Result<T>) => {
  return result.success ? { data: result.data } : { error: result.errors };
};