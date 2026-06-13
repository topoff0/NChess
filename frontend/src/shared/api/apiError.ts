type ApiErrorResponse = {
  code: string;
  description: string;
  errorType: number;
  validationErrors?: Record<string, string[]>;
};

export class ApiError extends Error {
  public readonly response: ApiErrorResponse;

  constructor(response: ApiErrorResponse) {
    super(getApiErrorMessage(response));
    this.name = "ApiError";
    this.response = response;
  }
}

export function getApiErrorMessage(error: ApiErrorResponse): string {
  const firstValidationError = Object.values(error.validationErrors ?? {})[0]?.[0];

  return firstValidationError ?? error.description ?? "Request failed";
}

export async function throwApiError(response: Response): Promise<never> {
  const error = (await response.json()) as ApiErrorResponse;

  throw new ApiError(error);
}
